using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.States;
using Stratus.Models.UI;

namespace Stratus.Godot.UI
{
	/// <summary>
	/// The menu to be used
	/// </summary>
	public abstract partial class GameMenu : CanvasLayer
	{
		[Export]
		private Container _container;

		protected Container container => _container;

		protected abstract Menu Generate();

		protected InputLayeredMenuGenerator menu { get; private set; }

		public override void _Ready()
		{
			base._Ready();
			GodotEventSystem.Connect<MenuEvent>(e => OnOpen());
			menu.onClose += GameState.Unset;
		}

		private void OnOpen()
		{
			Open(Generate());
		}

		protected void Open(Menu _menu)
		{
			this.Log($"Opening menu {_menu}");
			menu.Open(_menu);
		}
	}
}
