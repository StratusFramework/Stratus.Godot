using Godot;

using Stratus.Models.Maps;
using Stratus.Godot.Extensions;
using Stratus.Models.Actors;
using Stratus.Reflection;

namespace Stratus.Godot.Actors
{
	public partial class Object3D : Node3D, IObject3D
	{
		public System.Numerics.Vector3 position => this.Position.ToSystemVector3();

		public string name => this.Name;

		public override void _Ready()
		{
			base._Ready();
		}

		public void Interact(Actor3D source)
		{
			this.DispatchDown(new InteractEvent(source));
			this.Log($"{source} has interacted with me!");
		}
	}
}