using Godot;

using Stratus.Godot;
using Stratus.Godot.Extensions;
using Stratus.Models.Actors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototypes.Actors
{
	public abstract partial class TriggerableObject : Node
	{
		public override void _Ready()
		{
			base._Ready();
			this.Connect<InteractEvent>(e =>
			{
				this.Log("Triggering");
				OnInteractEvent(e);
			});
		}

		protected abstract void OnInteractEvent(InteractEvent e);
	}

	public partial class ToggleTriggerable : TriggerableObject
	{
		[Export]
		public Node target;

		protected override void OnInteractEvent(InteractEvent e)
		{
			target.Toggle();
		}
	}
}
