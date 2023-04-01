using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Godot.Tilemaps;
using Stratus.Models;
using Stratus.Models.Maps;
using Stratus.Numerics;
using Stratus.Search;
using Stratus.Utilities;

using System;

namespace Stratus.Godot.TileMaps
{
	public abstract partial class Map : Node2D
	{
		[Export]
		public TileMap tileMap;

		public GodotMap map { get; protected set; }
	}

	public abstract partial class Map<TLayer> : Map
		where TLayer : Enum
	{
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
	}

	public abstract partial class DefaultMap : Map<DefaultMapLayer>
	{	
	}
}
