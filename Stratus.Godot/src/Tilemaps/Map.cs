﻿using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Models;
using Stratus.Models.Maps;
using Stratus.Numerics;

using System;

namespace Stratus.Godot.TileMaps
{
	public abstract partial class Map : Node2D
	{
		[Export]
		public TileMap tileMap;

		public abstract GridRange GetRange(IActor2D actor);
	}

	public abstract partial class Map<TLayer> : Map
		where TLayer : Enum
	{
		public Grid2D<Node2D, TLayer> grid { get; private set; }

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
			this.LogInfo($"Initializing {tileMap}");
			var rect = tileMap.GetUsedRect();
			grid = new Grid2D<Node2D, TLayer>(new Vector2Int(rect.Size.X, rect.Size.Y), CellLayout.Rectangle);
		}

		public TNode? Get<TNode>(Vector2I position, TLayer layer) where TNode : Node2D 
		{
			return grid.Get<TNode>(layer, position.ToVector2Int());
		}

		public bool TryGet<TNode>(Vector2I position, TLayer layer, out TNode? obj) where TNode : Node2D
		{
			obj = grid.Get<TNode>(layer, position.ToVector2Int());
			if (obj == null)
			{
				return false;
			}
			return true;
		}
	}

	public abstract partial class DefaultMap : Map<DefaultMapLayer>
	{
		public override GridRange GetRange(IActor2D actor)
		{
			return grid.GetRange(DefaultMapLayer.Actor,
				(Node2D)actor, 5);
		}
	}

	public class TileMapLoadEvent : Event
	{
		public TileMapLoadEvent(TileMap tileMap)
		{
			this.tileMap = tileMap;
		}

		public TileMap tileMap { get; }
	}
}
