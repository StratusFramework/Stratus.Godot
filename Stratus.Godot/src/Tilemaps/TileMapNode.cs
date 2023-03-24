using Godot;

using Stratus.Godot.Extensions;

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

		public event Action<Vector2I> onMoved;

		public override void _Ready()
		{
		}

		public void Initialize(TileMap tileMap)
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
			bool moved = MoveTo(newPosition);
			return new Result<Vector2I>(moved,
				moved ? newPosition : cellPosition);
		}

		public void SnapToClosest()
		{
			var cellPosition = tileMap.LocalToMap(Position);
			MoveTo(cellPosition);
		}

		public bool MoveTo(Vector2I position)
		{
			if (!Contains(position))
			{
				return false;
			}

			onMoved?.Invoke(position);
			var localPos = tileMap.MapToLocal(position);
			Position = localPos;
			return true;
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
