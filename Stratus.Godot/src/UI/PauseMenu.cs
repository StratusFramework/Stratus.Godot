using Stratus.Events;
using Stratus.Models.Gameflow;
using Stratus.Models.UI;
using Stratus.src.Models.Games;

namespace Stratus.Godot.UI
{
	public partial class PauseMenu : GameStateMenu<PauseState>
	{
		public override void _Ready()
		{
			base._Ready();

			state.menu.Action("Resume", GameState.Exit);
			state.menu.Action("Help", () => StratusLog.Info("HELP!"), false);
			state.menu.Action("Quit", () =>
			{
				EventSystem.Broadcast(new EndGameEvent());
			});
		}

		protected override Menu Generate() => state.menu;
	}
}
