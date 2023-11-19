using Godot;

using Stratus.Godot.Audio;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Models.Audio;
using Stratus.Models.States;
using Stratus.Models.UI;

using System;

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

		private static readonly Type type = typeof(TState);

		protected abstract Menu Generate();

		protected InputLayeredMenuGenerator menu { get; private set; }

		public override void _Ready()
		{
			base._Ready();
			GameState.Enabled<TState>(Open);
			GameState.Disabled<TState>(Close);
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
