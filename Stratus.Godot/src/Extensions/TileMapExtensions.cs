using Godot;

namespace Stratus.Godot.Extensions
{
	public static class TileMapExtensions
	{
		public static TNode? InstantiateScene<TNode>(this TileMap tileMap, PackedScene scene, Vector2I pos)
			where TNode : Node2D
		{
			var instance = scene.Instantiate<TNode>();
			tileMap.AddChild(instance);
			var localPos = tileMap.MapToLocal(pos);
			instance.Position = localPos;
			return instance;
		}
	}

}
