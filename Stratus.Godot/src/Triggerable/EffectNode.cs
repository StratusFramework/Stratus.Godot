using Godot;

using Stratus.Models.Actors;

namespace Stratus.Godot.Triggers
{
	public abstract partial class EffectNode : Node
	{
		public override void _Ready()
		{
			base._Ready();
			this.Connect<InteractEvent>(e =>
			{
				OnInteractEvent(e);
			});
		}

		protected abstract void OnInteractEvent(InteractEvent e);
	}
}
