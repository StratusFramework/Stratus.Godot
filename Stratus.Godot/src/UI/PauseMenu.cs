using Stratus.Models.States;
using Stratus.Models.UI;

namespace Stratus.Godot.UI
{
	public partial class PauseMenu : GameStateMenu<PauseState>
	{
		protected override Menu Generate()
		{
			var menu = new Menu("Pause");
			menu.Item("Resume", StateStack.Exit);
			menu.Item("Help", () => StratusLog.Info("HELP!"), false);
			menu.Item("Quit", () =>
			{
				GodotEventSystem.Broadcast(new EndGameEvent());
			});
			return menu;
		}
	}
}
