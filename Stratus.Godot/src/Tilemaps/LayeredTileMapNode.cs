using Godot;

using System;

namespace Stratus.Godot.TileMaps
{
	public partial class LayeredTileMapNode<TLayer> : TileMapNode
		where TLayer : Enum
	{
		[Export]
		public TLayer layer;

		public override string ToString()
		{
			return $"[{layer}] {Name}";
		}
	}
}
