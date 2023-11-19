using Stratus.Models.States;
using Stratus.Models.UI;

namespace Stratus.Godot.UI
{
	public partial class PauseMenu : GameStateMenu<PauseState>
	{
		protected override Menu Generate()
		{
			var menu = new Menu("Pause");
			menu.Item("Resume", GameState.Pop);
			menu.Item("Help", () =>
			{

			});
			menu.Item("Quit", () => GameState.Return<MainMenuState>());
			return menu;
		}
	}
}
