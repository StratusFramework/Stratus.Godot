using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Models.Maps;

namespace Stratus.Godot.TileMaps
{
	public partial class CursorNode : TileMapNode
	{
		public override void _Ready()
		{
			EventSystem.Connect<MoveCursorEvent>(e =>
			{
				if (e.range != null)
				{
					var next = cellPosition + e.direction.ToVector2I();
					if (!e.range.ContainsKey(next.ToVector2Int()))
					{
						return;
					}
				}

				var move = Move(e.direction.ToVector2I());
				OnMoved(move);
			});

			EventSystem.Connect<SetCursorEvent>(e =>
			{
				var move = MoveTo(e.position.ToVector2I());
				OnMoved(move);
			});
		}

		private static void OnMoved(Result<Vector2I> move)
		{
			if (move)
			{
				EventSystem.Broadcast(new CursorMovedEvent(move.result.ToVector2Int()));
			}
		}
	}

	
}
