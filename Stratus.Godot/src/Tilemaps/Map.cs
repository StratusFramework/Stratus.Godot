using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Models;
using Stratus.Models.Maps;
using Stratus.Numerics;
using Stratus.Utilities;

using System;
using System.Xml.Linq;

namespace Stratus.Godot.TileMaps
{
	public abstract partial class Map : Node2D
	{
		[Export]
		public TileMap tileMap;

		public abstract GridRange GetRange(IActor2D actor);
	}

	public class TileInfo : IEquatable<TileInfo>
	{
		public TileInfo(TileData data, Vector2I position)
		{
			this.data = data;
			this.position = position;
		}

		public TileInfo(Node2D node)
		{
			this.node = node;
		}

		/// <summary>
		/// For active objects on a tile
		/// </summary>
		public Node2D node { get; }
		/// <summary>
		/// For static tiles (painted from a tileset)
		/// </summary>
		public TileData data { get; }
		/// <summary>
		/// Used in conjuntion with <see cref="data"/> to differentiate
		/// since data is shared among all tiles of a given type
		/// </summary>
		public Vector2I position { get; }

		public bool Equals(TileInfo? other)
		{
			return this.node == other.node && this.data == other.data
				&& this.position == other.position;
		}

		public override bool Equals(object? obj) => Equals(obj as TileInfo);

		public override int GetHashCode()
		{
			return node?.GetHashCode() ?? data.GetHashCode();
		}

		public override string ToString()
		{
			return node != null ? node.ToString() : data.ToString();
		}
	}

	public abstract partial class Map<TLayer> : Map
		where TLayer : Enum
	{
		public Grid2D<TileInfo, TLayer> grid { get; private set; }

		public override void _Ready()
		{
			tileMap = this.GetChildOfType<TileMap>();
			if (tileMap != null)
			{
				Initialize(tileMap);
			}
		}

		public override string ToString()
		{
			return $"<{Name}:{tileMap}>";
		}

		public virtual void Initialize(TileMap tileMap)
		{
			this.LogInfo($"Initializing {tileMap.Name}");
			var rect = tileMap.GetUsedRect();
			grid = new Grid2D<TileInfo, TLayer>(new Vector2Int(rect.Size.X, rect.Size.Y), CellLayout.Rectangle);

			var layerCount = tileMap.GetLayersCount();
			for (int l = 0; l < layerCount; l++)
			{
				string name = tileMap.GetLayerName(l);
				TLayer layer = EnumUtility.Value<TLayer>(name);
				var positions = tileMap.GetUsedCells(l);
				this.LogInfo($"Found {positions.Count} cells used in layer {name}");
				foreach (var pos in positions)
				{
					TileData data = tileMap.GetCellTileData(l, pos);
					var result = grid.Set(layer, new TileInfo(data, pos), pos.ToVector2Int());
					if (!result)
					{
						StratusLog.Result(result);
					}
				}
				this.LogInfo($"Set {grid.Count(layer)} tile(s) on the {layer} layer");
			}
		}

		public TNode? Get<TNode>(Vector2I position, TLayer layer) where TNode : Node2D
		{
			var info = grid.Get(layer, position.ToVector2Int());
			if (info == null)
			{
				return null;
			}
			return (TNode)info.node;
		}

		public bool TryGet<TNode>(Vector2I position, TLayer layer, out TNode? obj) where TNode : Node2D
		{
			obj = (TNode)grid.Get(layer, position.ToVector2Int()).node;
			if (obj == null)
			{
				return false;
			}
			return true;
		}
	}

	public abstract partial class DefaultMap : Map<DefaultMapLayer>
	{
		public override GridRange GetRange(IActor2D actor)
		{
			return grid.GetRange(DefaultMapLayer.Actor,
				new TileInfo((Node2D)actor),
				new GridSearchRangeArguments(0, actor.range)
				{
					traversableFunction = pos =>
					{
						if (!grid.Contains(DefaultMapLayer.Terrain, pos))
						{
							StratusLog.Info($"No terrain at {pos} ({grid.Count(DefaultMapLayer.Terrain)})");
							return Search.TraversableStatus.Invalid;
						}

						if (grid.Contains(DefaultMapLayer.Wall, pos))
						{
							StratusLog.Info($"Wall at {pos}");
							return Search.TraversableStatus.Blocked;
						}

						if (grid.Contains(DefaultMapLayer.Actor, pos))
						{
							StratusLog.Info($"Actor already at {pos}");
							return Search.TraversableStatus.Occupied;
						}

						return Search.TraversableStatus.Valid;
					}
				});
		}
	}

	public class TileMapLoadEvent : Event
	{
		public TileMapLoadEvent(TileMap tileMap)
		{
			this.tileMap = tileMap;
		}

		public TileMap tileMap { get; }
	}
}
