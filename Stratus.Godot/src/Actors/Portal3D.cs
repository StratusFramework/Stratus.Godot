using Stratus.Models.Maps;

using System;

namespace Stratus.Godot.Actors
{
	public interface IPortal3D : IPortal, IObject3D
	{
	}

	public partial class Portal3D : Object3D, IPortal3D
	{
		public bool open { get; set; }
		public string name { get; }
		public bool locked { get; }

		public event Action<bool> onToggle;

		public Portal3D(string name)
		{
			this.name = name;
		}

		public void Toggle()
		{
			open = !open;
		}
	}
}