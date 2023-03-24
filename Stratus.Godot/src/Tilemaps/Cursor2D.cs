using Godot;

using Stratus.Events;

namespace Stratus.Godot.TileMaps
{
	public partial class Cursor2D : TileMapNode
	{
		public override void _Ready()
		{
			GodotEventSystem.Connect<MoveCursorEvent>(e =>
			{
				var move = Move(e.direction);
				if (move)
				{
					GodotEventSystem.Broadcast(new CursorMovedEvent(move));
				}
			});
		}
	}

	public class MoveCursorEvent : Event
	{
		public MoveCursorEvent(Vector2I direction)
		{
			this.direction = direction;
		}

		public Vector2I direction { get; }
	}

	public class CursorMovedEvent : Event
	{
		public Vector2I position { get; }

		public CursorMovedEvent(Vector2I position)
		{
			this.position = position;
		}
	}

	public class SelectCursorEvent : Event
	{
		public SelectCursorEvent()
		{
		}
	}

	public class TileSelectedEvent : Event
	{
	}
}
