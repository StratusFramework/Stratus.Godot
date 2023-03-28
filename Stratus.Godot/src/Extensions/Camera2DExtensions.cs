using Godot;

using Stratus.Numerics;

namespace Stratus.Godot.Extensions
{
	public static class Camera2DExtensions
    {
		public static void Limit(this Camera2D camera, TileMap map)
		{
			var mapRect = map.GetUsedRect();
			var tileSize = map.CellQuadrantSize;
			var worldSizeInPixels = mapRect.Size * tileSize;
			camera.LimitLeft = 0;
			camera.LimitTop = 0;
			camera.LimitRight = worldSizeInPixels.X;
			camera.LimitBottom = worldSizeInPixels.Y;
		}
		
		public static string LimitsToString(this Camera2D camera)
		{
			return $"X({camera.LimitLeft}, {camera.LimitRight}), Y({camera.LimitBottom}, {camera.LimitTop})";
		}
	}
}
