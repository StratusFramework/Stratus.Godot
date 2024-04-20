using Godot;

using Stratus.Events;
using Stratus.Inputs;
using Stratus.Models.Gameflow;
using Stratus.src.Models.Games;
using System;

using static Stratus.Inputs.InputLayer;

namespace Stratus.Godot.Inputs
{
	public abstract class GodotInputLayer<TAction> : InputLayer<InputEvent, GodotInputActionMapHandler>
		where TAction : Enum
	{
		public GodotInputLayer(string name = null) : base(name ?? typeof(TAction).Name,
			new GodotInputActionMapHandler(typeof(TAction).Name))
		{
		}
	}

	public static class GodotInputLayerExtensions
	{
		public static void Push(this InputLayer layer)
		{
			EventSystem.Broadcast(new PushEvent(layer));
		}

		public static void Pop(this InputLayer layer)
		{
			EventSystem.Broadcast(new PopEvent(layer));
		}
	}

	public enum MovementInputAction
	{
		move_left,
		move_right,
		move_up,
		move_down
	}

	/// <summary>
	/// Used to hold axis input state during polling
	/// </summary>
	public class AxisInputState
	{
		public bool left;
		public bool right;
		public bool up;
		public bool down;

		public Vector2 xy
		{
			get
			{
				float x = 0;

				if (right)
				{
					x = 1f;
				}
				else if (left)
				{
					x = -1f;
				}

				float y = 0;
				if (up)
				{
					y = -1f;
				}
				else if (down)
				{
					y = 1f;
				}
				return new Vector2(x, y);
			}
		}

		public Vector3 xz
		{
			get
			{
				var input = xy;
				return new Vector3(input.X, 0, input.Y);
			}
		}
	}

	public enum GodotInputAction
	{
		ui_accept,
		ui_select,
		ui_cancel,
		ui_left,
		ui_right,
		ui_up,
		ui_down
	}

	public abstract class GodotInputGameState<TInputLayer> : InputState<TInputLayer>
		where TInputLayer : InputLayer, new()
	{
		public override void Enter()
		{
			base.Enter();
			inputLayer.Push();
		}

		public override void Exit()
		{
			base.Exit();
			inputLayer.Pop();
		}
	}
}
