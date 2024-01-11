using Godot;

using Stratus.Models.Gameflow;
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

			state.menu.Child("Gameplay", menu =>
			{

				menu.Close("Back");
			});

			state.menu.Child("Graphics", menu =>
			{
				menu.Option("Window Mode", ObjectReference.Enum(
					() => DisplayServer.WindowGetMode(),
					m => DisplayServer.WindowSetMode(m)));

				menu.Option("Vsync",
					ObjectReference.Enum(
						() => DisplayServer.WindowGetVsyncMode(),
						mode => DisplayServer.WindowSetVsyncMode(mode)));

				menu.Close("Back");

			});
						
			state.menu.Child("Audio", menu =>
			{
				menu.BusVolume("Master");
				menu.Option("Subtitles", ObjectReference.Boolean(() => subtitles,
					v => subtitles = v));
				menu.Close("Back");
			});
			state.menu.Action("Back", GameState.Exit);
		}

		protected override Menu Generate() => state.menu;
	}

	public static class OptionsMenuExtensions
	{
		public static MenuOption BusVolume(this Menu menu, string bus)
		{
			int masterAudioBusIndex = AudioServer.GetBusIndex(bus);
			var option = new MenuOption($"{bus} Volume",
					() => Mathf.DbToLinear(AudioServer.GetBusVolumeDb(masterAudioBusIndex)),
					v =>
					{
						AudioServer.SetBusVolumeDb(masterAudioBusIndex, Mathf.LinearToDb(v));
					}, 0f, 1f);
			menu.Option(option);
			return option;
		}
	}
}
