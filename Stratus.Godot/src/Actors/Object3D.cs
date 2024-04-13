using Godot;

using Stratus.Models.Maps;
using Stratus.Godot.Extensions;

namespace Stratus.Godot.Actors
{
	public partial class Object3D : Node3D, IObject3D
	{
		public System.Numerics.Vector3 position => this.Position.ToSystemVector3();
	}
}