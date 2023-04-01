using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Models;

namespace Stratus.Godot.TileMaps
{
	public partial class Cursor2D : TileMapNode
	{
		public override void _Ready()
		{
			GodotEventSystem.Connect<MoveCursorEvent>(e =>
			{
				if (e.range != null)
				{
					var next = cellPosition + e.direction;
					if (!e.range.ContainsKey(next.ToVector2Int()))
					{
						return;
					}
				}

				var move = Move(e.direction);
				OnMoved(move);
			});

			GodotEventSystem.Connect<SetCursorPositionEvent>(e =>
			{
				var move = MoveTo(e.position);
				OnMoved(move);
			});
		}

		private static void OnMoved(Result<Vector2I> move)
		{
			if (move)
			{
				GodotEventSystem.Broadcast(new CursorMovedEvent(move));
			}
			else
			{
				StratusLog.Result(move);
			}
		}
	}

	public class MoveCursorEvent : Event
	{
		public Vector2I direction { get; }
		public GridRange range { get; }

		public MoveCursorEvent(Vector2I direction)
		{
			this.direction = direction;
		}

		public MoveCursorEvent(Vector2I direction, GridRange range) : this(direction)
		{
			this.range = range;
		}
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

	public class SetCursorPositionEvent : Event
	{
		public SetCursorPositionEvent(Vector2I position)
		{
			this.position = position;
		}

		public Vector2I position { get; }
	}
}
