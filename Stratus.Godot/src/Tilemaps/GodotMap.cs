using Godot;

using Stratus.Data;
using Stratus.Godot.Extensions;
using Stratus.Godot.TileMaps;
using Stratus.Models.Maps;
using Stratus.Numerics;
using Stratus.Utilities;

using System.Xml.Linq;

namespace Stratus.Godot.Tilemaps
{
	public class GodotMap : Map2D<TileData>
	{
		public GodotMap(TileMap tileMap) : base(() => Generate(tileMap))
		{
		}

		private static Grid Generate(TileMap tileMap)
		{
			var rect = tileMap.GetUsedRect();
			var size = new Vector2Int(rect.Size.X, rect.Size.Y);
			var grid = new Grid(size, CellLayout.Rectangle);

			// Set the terrain, wall and object? layers
			var layerCount = tileMap.GetLayersCount();
			for (int l = 0; l < layerCount; l++)
			{
				string name = tileMap.GetLayerName(l);
				DefaultMapLayer layer = EnumUtility.Value<DefaultMapLayer>(name);
				var positions = tileMap.GetUsedCells(l);
				foreach (var pos in positions)
				{
					TileData data = tileMap.GetCellTileData(l, pos);
					var _pos = pos.ToVector2Int();
					var result = grid.Set(layer, new TileInfo(data, _pos), _pos);
					if (!result)
					{
						StratusLog.Result(result);
					}
				}
			}

			return grid;
		}
	}

	public class TileInfo : CellReference<IObject2D, TileData>
	{
		public TileInfo(ValueProvider<IObject2D> provider) : base(provider)
		{
		}

		public TileInfo(TileData data, Vector2Int position)
			: base(data, position)
		{
		}
	}

	public class GodotTileMapReference : TileMapReference<TileMap>
	{
		public GodotTileMapReference(TileMap tileMap) : base(tileMap)
		{
		}

		public override Vector2Int size
		{
			get
			{
				var rect = tileMap.GetUsedRect();
				return new Vector2Int(rect.Size.X, rect.Size.Y);
			}
		}

		public override int layerCount => tileMap.GetLayersCount();
	}
}
