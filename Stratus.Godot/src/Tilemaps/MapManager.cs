using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;

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

	public abstract partial class MapManager : Node2D
	{
		public class LoadEvent : TileMapEvent
		{
			public LoadEvent(TileMap tileMap) : base(tileMap)
			{
			}
		}
	}

	public abstract partial class MapManager<TMap> : MapManager
		where TMap : Map
	{
		public abstract Cursor2D cursor { get; }
		public abstract Camera2D camera { get; }
		public TMap map { get; protected set; }
		public bool initialized { get; private set; }

		private MapInputLayer inputLayer = new MapInputLayer();

		#region Abstract
		protected abstract void OnReady();
		protected abstract void OnCursorMovedEvent(CursorMovedEvent e);
		public abstract void SelectAtCursor();
		#endregion

		#region Engine
		public override void _Ready()
		{
			GodotEventSystem.Connect<CursorMovedEvent>(OnCursorMovedEvent);
			GodotEventSystem.Connect<SelectCursorEvent>(e =>
			{
				SelectAtCursor();
			});

			if (camera == null)
			{
				StratusLog.Error("No camera has been set");
			}

			OnReady();
		}

		public override void _Process(double delta)
		{
			camera.GlobalPosition = cursor.GlobalPosition;
		}
		#endregion

		public void Load()
		{
			Load(map ?? this.GetChildOfType<TMap>());
		}

		public void Load(TMap map)
		{
			if (map == null)
			{
				this.LogWarning("No map to load");
				return;
			}

			this.LogInfo($"Loading the map {map}");
			this.map = map;
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
				GodotEventSystem.Broadcast(new MoveCursorEvent(dir));
			});
			map.Bind(MapInputAction.Select, () =>
			{
				GodotEventSystem.Broadcast(new SelectCursorEvent());
			});
		}
	}
}
