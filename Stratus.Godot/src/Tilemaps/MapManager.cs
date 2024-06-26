﻿using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Models.Maps;

using System;
using System.Linq;

namespace Stratus.Godot.TileMaps
{
	public abstract record TileMapEvent : Event
	{
		public TileMap tileMap { get; }

		protected TileMapEvent(TileMap tileMap)
		{
			this.tileMap = tileMap;
		}
	}

	public abstract record MapEvent : Event
	{
		public Vector2I position { get; }

		protected MapEvent(Vector2I position)
		{
			this.position = position;
		}
	}

	public abstract partial class MapManager<TMapNode> : MapManager
		where TMapNode : MapNode
	{
		public abstract CursorNode cursor { get; }
		public abstract Camera2D camera { get; }
		public TMapNode mapNode { get; protected set; }
		public override IMap2D map => mapNode.map;
		public override TileMap? tileMap => mapNode != null ? mapNode.tileMap : null;
		public bool initialized { get; private set; }
		public override Vector2I cursorPosition => cursor.cellPosition;

		#region Events
		public event Action onLoad;
		public event Action onUnload;
		#endregion

		#region Abstract
		protected abstract void OnCursorMovedEvent(CursorMovedEvent e);
		public abstract void SelectAtCursor();
		#endregion

		#region Engine
		public override void _Ready()
		{
			EventSystem.Connect<CursorMovedEvent>(e =>
			{
				// Check if input active or...?
				OnCursorMovedEvent(e);
			});
			EventSystem.Connect<SelectCursorEvent>(e => SelectAtCursor());

			if (camera == null)
			{
				StratusLog.Error("No camera has been set");
			}

			foreach (var gizmo in this.GetChildrenOfType<ManagedMapNode>())
			{
				gizmo.Initialize(this);
			}
		}

		public override void _Process(double delta)
		{
			camera.GlobalPosition = cursor.GlobalPosition;
		}
		#endregion

		#region Loading
		public void Load(TMapNode node)
		{
			if (node == null)
			{
				this.LogWarning("No map to load");
				return;
			}

			this.mapNode = node;
			this.Log($"Initializing the map node {node}");

			var initialPosition = node.map.grid.Cells(DefaultMapLayer.Terrain).First();			
			cursor.Attach(node.tileMap);
			cursor.MoveTo(initialPosition);
			initialized = true;
			node.Visible = true;
			onLoad?.Invoke();
		}

		public void Load(PackedScene scene)
		{
			this.Log($"Instancing the map scene {scene}");
			var map = this.InstantiateScene<TMapNode>(scene);
			Load(map);
		}

		public void Unload()
		{
			this.Log($"Unloading the map");

			cursor.Detach();
			onUnload?.Invoke();
			initialized = false;
		}
		#endregion
	}

	// TODO: Refactor between setup map movement and encounter map movement

}
