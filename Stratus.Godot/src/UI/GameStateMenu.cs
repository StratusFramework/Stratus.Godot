using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.States;
using Stratus.Models.UI;

namespace Stratus.Godot.UI
{
	/// <summary>
	/// The menu to be used
	/// </summary>
	public abstract partial class GameStateMenu<TState> : CanvasLayer
		where TState : GameState
	{
		[Export]
		private Container _container;

		protected Container container => _container;
		protected TState state => StateStack.Get<TState>();

		protected abstract Menu Generate();

		protected InputLayeredMenuGenerator menu { get; private set; }

		public override void _Ready()
		{
			base._Ready();
			StateStack.Entered<TState>(this, Open);
			StateStack.Exited<TState>(this, Close);
			menu = new InputLayeredMenuGenerator(this, container);
			Visible = false;
		}

		private void OnOpen()
		{
			Open(Generate());
		}

		protected void Open(Menu _menu)
		{
			if (menu.opened)
			{
				return;
			}
			this.Log($"Opening");
			menu.Open(_menu);
		}

		public virtual void Open() => Open(Generate());

		public virtual void Close()
		{
			if (!menu.opened)
			{
				return;
			}
			menu.Close();
			this.Log($"Closing");
		}
	}
}
