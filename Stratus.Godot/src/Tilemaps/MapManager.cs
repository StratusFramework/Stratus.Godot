using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Models;
using Stratus.Models.Maps;
using Stratus.Models.Maps.Actions;

namespace Stratus.Godot.TileMaps
{
	public abstract class TileMapEvent : Event
	{
		public TileMap tileMap { get; }

		protected TileMapEvent(TileMap tileMap)
		{
			this.tileMap = tileMap;
		}
	}

	public abstract class MapEvent : Event
	{
		public Vector2I position { get; }

		protected MapEvent(Vector2I position)
		{
			this.position = position;
		}
	}

	public interface IMapManager
	{
		TileMap? tileMap { get; }
		Vector2I cursorPosition { get; }
		IMap2D map { get; }
	}

	public abstract partial class MapManager : Node2D, IMapManager
	{
		public abstract TileMap? tileMap { get; }
		public abstract Vector2I cursorPosition { get; }
		public abstract IMap2D map { get; }

		public class LoadEvent : TileMapEvent
		{
			public LoadEvent(TileMap tileMap) : base(tileMap)
			{
			}
		}

		public override string ToString() => Name;
	}

	public abstract partial class MapManager<TMap> : MapManager
		where TMap : Map
	{
		public abstract Cursor2D cursor { get; }
		public abstract Camera2D camera { get; }
		public TMap mapNode { get; protected set; }
		public override IMap2D map => mapNode.map;
		public override TileMap? tileMap => mapNode != null ? mapNode.tileMap : null;
		public bool initialized { get; private set; }
		public override Vector2I cursorPosition => cursor.cellPosition;


		private MapInputLayer inputLayer = new MapInputLayer();

		#region Abstract
		protected abstract void OnReady();
		protected abstract void OnCursorMovedEvent(CursorMovedEvent e);
		public abstract void SelectAtCursor();
		#endregion

		#region Engine
		public override void _Ready()
		{
			GodotEventSystem.Connect<CursorMovedEvent>(e =>
			{
				if (inputLayer.active)
				{
					OnCursorMovedEvent(e);
				}
			});
			GodotEventSystem.Connect<SelectCursorEvent>(e => SelectAtCursor());
			GodotEventSystem.Connect<MovementRangeEvent.Request>(e =>
			{
				var range = map.GetRange(e.input);
				if (range != null)
				{
					GodotEventSystem.Broadcast(new MovementRangeEvent.Response(e.input, range));
				}
				else
				{
					this.LogError($"Could not find the range for {e.input}");
				}
			});

			if (camera == null)
			{
				StratusLog.Error("No camera has been set");
			}

			foreach (var gizmo in this.GetChildrenOfType<MapNode>())
			{
				gizmo.Initialize(this);
			}

			OnReady();
		}

		public override void _Process(double delta)
		{
			camera.GlobalPosition = cursor.GlobalPosition;
		}
		#endregion

		#region Loading
		public void Load()
		{
			Load(mapNode ?? this.GetChildOfType<TMap>());
		}

		public void Load(TMap map)
		{
			if (map == null)
			{
				this.LogWarning("No map to load");
				return;
			}

			this.LogInfo($"Loading the map {map}");
			this.mapNode = map;
			cursor.Initialize(map.tileMap);
			cursor.MoveTo(Vector2I.Zero);
			inputLayer.Push();
			initialized = true;
		}

		public void Unload()
		{
			this.LogInfo($"Unloading the map");

			cursor.Disable();
			inputLayer.Pop();
			initialized = false;
		}
		#endregion
	}

	public enum MapInputAction
	{
		Move,
		Select,
		Cancel,
		Menu,
		Pause
	}

	public class MapInputLayer : GodotInputLayer<MapInputAction>
	{
		protected override void Initialize()
		{
			map.Bind(MapInputAction.Move, value =>
			{
				var dir = new Vector2I((int)value.X, -(int)value.Y);
				GodotEventSystem.Broadcast(new MoveCursorEvent(dir.ToVector2Int()));
			});

			map.Bind(MapInputAction.Select, () =>
			{
				GodotEventSystem.Broadcast(new SelectCursorEvent());
			});
		}
	}
}
