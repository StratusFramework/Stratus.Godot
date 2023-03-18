using Godot;

namespace Stratus.Godot
{
	/// <summary>
	/// A node that works on top of an existing <see cref="TileMap"/>
	/// </summary>
	public partial class TileMapNode2D : Node2D
	{
		public TileMap tileMap { get; private set; }

		public void Initialize(TileMap tileMap)
		{
			this.tileMap = tileMap;
		}

		public void Move(Vector2I direction)
		{
			var cellPosition = tileMap.LocalToMap(Position);
			cellPosition += direction;
			Snap(cellPosition);
		}

		public void Snap(Vector2I position)
		{
			if (!Contains(position))
			{
				return;
			}
			var localPos = tileMap.MapToLocal(position);
			Position = localPos;
		}

		public bool Contains(Vector2I position)
		{
			return tileMap.GetCellTileData(0, position) != null;
		}
	}
}
