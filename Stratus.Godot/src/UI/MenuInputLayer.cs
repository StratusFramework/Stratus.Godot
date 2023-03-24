using Godot;

using System;
using Stratus.Godot.Inputs;

namespace Stratus.Godot.UI
{
	public class MenuInputLayer : GodotInputLayer<MenuInputAction>
	{
		public Action<Vector2I> move;
		public Action select;
		public Action cancel;

		public MenuInputLayer() : base()
		{
		}

		protected override void Initialize()
		{
			map.TryBindAll<MenuInputAction>();
		}
	}

	public enum MenuInputAction
	{
		Move,
		Select,
		Cancel,
	}
}
