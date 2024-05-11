using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.Actors;

namespace Stratus.Godot.Triggers
{
	/// <summary>
	/// Toggles the target <see cref="Node"/>
	/// </summary>
	public partial class ToggleEffectNode : EffectNode
	{
		[Export]
		public Node target;

		protected override void OnInteractEvent(InteractEvent e)
		{
			target.Toggle();
		}
	}
}
