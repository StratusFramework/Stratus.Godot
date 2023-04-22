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
		//Vector2Int IObject2D.cellPosition => new Vector2Int(cellPosition.X, cellPosition.Y);
		//string IObject2D.name => Name;

		public event Action<Vector2Int> onMoved;

		public override void _Ready()
		{
		}

		public virtual void Initialize(TileMap tileMap)
		{
			this.tileMap = tileMap;
			this.Reparent(tileMap);
			SnapToClosest();
		}

		public void Disable()
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

			onMoved?.Invoke(position.ToVector2Int());
			var localPos = tileMap.MapToLocal(position);
			Position = localPos;
			return new Result<Vector2I>(true, position);
		}

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
