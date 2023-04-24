using Godot;

using Stratus.Extensions;
using Stratus.Godot.Extensions;
using Stratus.Godot.Tilemaps;

using System;
using System.Collections.Generic;

namespace Stratus.Godot.TileMaps
{
	public abstract partial class MapNode : Node2D
	{
		[Export]
		public TileMap tileMap;

		public GodotMap map { get; protected set; }
		public bool initialized { get; private set; }

		public event Action onInitialized;

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
			this.Log("Initializing tilemap...");
			map = new GodotMap(tileMap);
		}

		public static IEnumerable<TNode> InitializeAll<TNode>(Node2D node, TileMap tilemap, Action<TNode> action)
			where TNode : TileMapNode
		{
			return node.GetChildrenOfType<TNode>().ForEach(n =>
			{
				n.Attach(tilemap);
				action(n);
			});
		}
	}
}
