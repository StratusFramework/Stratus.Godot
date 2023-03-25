using Godot;

using Stratus.Inputs;

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
			GodotEventSystem.Broadcast(new PushEvent(layer));
		}

		public static void Pop(this InputLayer layer)
		{
			GodotEventSystem.Broadcast(new PopEvent(layer));
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
}
