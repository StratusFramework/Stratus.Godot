using Godot;

using Stratus.Extensions;
using Stratus.Godot.Extensions;
using Stratus.Godot.Tilemaps;

using System;

namespace Stratus.Godot.TileMaps
{
	public abstract partial class MapNode : Node2D
	{
		[Export]
		public TileMap tileMap;

		public GodotMap map { get; protected set; }

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
			map = new GodotMap(tileMap);
		}

		public static void InitializeAll<TNode>(Node2D node, TileMap tilemap, Action<TNode> action)
			where TNode : TileMapNode
		{
			node.GetChildrenOfType<TNode>().ForEach(n =>
			{
				n.Initialize(tilemap);
				action(n);
			});
		}
	}
}
