using Godot;

using Stratus.Inputs;

using System;

namespace Stratus.Godot.Inputs
{
	public abstract class GodotInputLayer<TAction> : InputLayer<InputEvent, GodotInputActionMapHandler>
		where TAction : Enum
	{
		public GodotInputLayer() : base(typeof(TAction).Name, new GodotInputActionMapHandler(typeof(TAction).Name))
		{
			Initialize();
		}

		protected abstract void Initialize();

		public override bool HandleInput(InputEvent input)
		{
			return map.HandleInput(input);
		}

		public void Push()
		{
			GodotEventSystem.Broadcast(new PushEvent(this));
		}

		public void Pop()
		{
			GodotEventSystem.Broadcast(new PopEvent(this));
		}
	}
}
