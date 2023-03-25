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

		public MenuInputLayer(string name) : base(name)
		{
		}

		protected override void Initialize()
		{
			map.Bind(MenuInputAction.Move.ToString(), Move);
			map.Bind(MenuInputAction.Select, Select);
			map.Bind(MenuInputAction.Cancel, Cancel);
		}

		private void Move(Vector2I value) => move?.Invoke(value);
		private void Select() => select?.Invoke();
		private void Cancel() => cancel?.Invoke();	
	}

	public enum MenuInputAction
	{
		Move,
		Select,
		Cancel,
	}
}
