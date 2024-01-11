using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.Gameflow;
using Stratus.Models.UI;
using Stratus.src.Models.Games;

namespace Stratus.Godot.UI
{
    /// <summary>
    /// The menu to be used
    /// </summary>
    public abstract partial class GameStateMenu<TState> : CanvasLayer
		where TState : State
	{
		[Export]
		private Container _container;

		protected Container container => _container;
		protected TState state => GameState.Get<TState>();

		protected abstract Menu Generate();

		protected InputLayeredMenuGenerator menu { get; private set; }

		public override void _Ready()
		{
			base._Ready();
			GameState.Entered<TState>(this, Open);
			GameState.Exited<TState>(this, Close);
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
