using Godot;

namespace Stratus.Godot
{
	/// <summary>
	/// A node that works on top of an existing <see cref="TileMap"/>
	/// </summary>
	public partial class TileMapNode2D : Node2D
	{
		public TileMap tileMap { get; private set; }
		public bool ready => tileMap != null;

		public override void _Ready()
		{
			
		}

		public void Initialize(TileMap tileMap)
		{
			this.tileMap = tileMap;
			this.Reparent(tileMap);
		}

		public void Disable()
		{
			tileMap = null;
			Reparent(null);
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
			//var worldPos = ToGlobal(localPos);
			Position = localPos;
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
