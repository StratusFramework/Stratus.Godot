using Godot;

using Stratus.Godot.Extensions;
using Stratus.Numerics;

using System;

namespace Stratus.Godot.TileMaps
{
	/// <summary>
	/// A node that works on top of an existing <see cref="TileMap"/>
	/// </summary>
	public partial class TileMapNode : Node2D
	{
		public TileMap tileMap { get; private set; }
		public bool ready => tileMap != null;
		public Vector2I cellPosition => tileMap.LocalToMap(Position);

		public event Action<Vector2Int> onMoved;

		public override void _Ready()
		{
		}

		public override string ToString()
		{
			return Name;
		}

		public virtual void Attach(TileMap tileMap)
		{
			StratusLog.AssertNotNull(tileMap, "No tilemap was given");
			this.tileMap = tileMap;
			Reparent(tileMap);
			SnapToClosest();
			this.Log($"Attached {this} to tilemap {tileMap}");
		}

		public void Detach()
		{
			tileMap = null;
			Reparent(null);
		}

		public Result<Vector2I> Move(Vector2I direction)
		{
			var newPosition = cellPosition + direction;
			var moved = MoveTo(newPosition);
			return moved;
		}

		public void SnapToClosest()
		{
			var cellPosition = tileMap.LocalToMap(Position);
			MoveTo(cellPosition);
		}

		public Result<Vector2I> MoveTo(Vector2I position)
		{
			if (!Contains(position))
			{
				return new Result<Vector2I>(false, cellPosition, $"Cannot move to position {position} as it does not exist");
			}

			var localPos = tileMap.MapToLocal(position);			
			Position = localPos;
			onMoved?.Invoke(position.ToVector2Int());
			return new Result<Vector2I>(true, position, $"Moved {this} to {position}");
		}

		public Result<Vector2I> MoveTo(Vector2Int position) => MoveTo(position.ToVector2I());

		public bool Contains(Vector2I position)
		{
			if (!ready)
			{
				StratusLog.Error("No tilemap set");
				return false;
			}
			return tileMap.GetCellTileData(0, position) != null;
		}
	}
}
