using Godot;

using Stratus.Models.States;
using Stratus.Models.UI;
using Stratus.Reflection;

namespace Stratus.Godot.UI
{
	public partial class OptionsMenu : GameStateMenu<OptionsMenuState>
	{
		// TODO: Move
		private bool subtitles;

		public override void _Ready()
		{
			base._Ready();
			state.menu.SubMenu("Graphics", menu =>
			{		
				menu.Option("Window Mode",
					new ObjectReference<DisplayServer.WindowMode>(
						() => DisplayServer.WindowGetMode(),
						m => DisplayServer.WindowSetMode(m)));
				menu.Close("Back");
				
			});
			state.menu.SubMenu("Audio", menu =>
			{
				menu.Option("Subtitles", ObjectReference.Boolean(() => subtitles, v => subtitles = v));
				menu.Close("Back");
			});
			state.menu.Item("Back", StateStack.Exit);
		}

		protected override Menu Generate() => state.menu;
	}
}
