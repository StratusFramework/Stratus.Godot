using Godot;

using Stratus.Godot.Audio;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Models;
using Stratus.Models.Audio;
using Stratus.Models.Gameflow;
using Stratus.Models.States;
using Stratus.Models.UI;
using Stratus.src.Models.Games;

namespace Stratus.Godot.UI
{
    public partial class MainMenu : GameStateMenu<MainMenuState>
	{
		#region Messages		
		public override void Open()
		{
			base.Open();
			this.Invoke(() => SoundtrackPlayer.Play(new PlayAudioEvent(DefaultAudioChannel.Background, "mainmenu")), 1);
		}

		public override void Close()
		{
			base.Close();
			SoundtrackPlayer.Stop(new StopAudioEvent(DefaultAudioChannel.Background));
		}

		protected override Menu Generate()
		{
			var menu = new Menu("Main Menu");
			menu.Action("Start", Start);
			menu.Action("Options", Options);
			menu.Action("Quit", Quit);
			menu.NotClosable();
			return menu;
		}
		#endregion

		#region Menu Items
		private void Start()
		{
			Close();
			this.Log("Starting game");
			GodotEventSystem.Broadcast(new StartGameEvent());
		}

		private void Options()
		{
			GameState.Enter<OptionsMenuState>();
		}

		private void Quit()
		{
			this.Log("Quitting game");
			GetTree().Quit();
		}
		#endregion
	}
}
