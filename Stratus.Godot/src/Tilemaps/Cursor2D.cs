using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Models;
using Stratus.Models.Maps;
using Stratus.Numerics;

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
					var next = cellPosition + e.direction.ToVector2I();
					if (!e.range.ContainsKey(next.ToVector2Int()))
					{
						return;
					}
				}

				var move = Move(e.direction.ToVector2I());
				OnMoved(move);
			});

			GodotEventSystem.Connect<SetCursorEvent>(e =>
			{
				var move = MoveTo(e.position.ToVector2I());
				OnMoved(move);
			});
		}

		private static void OnMoved(Result<Vector2I> move)
		{
			if (move)
			{
				GodotEventSystem.Broadcast(new CursorMovedEvent(move.result.ToVector2Int()));
			}
			//else
			//{
			//	StratusLog.Result(move);
			//}
		}
	}

	
}
