using Godot;

using Stratus.Godot.Audio;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Models;
using Stratus.Models.Audio;
using Stratus.Models.States;
using Stratus.Models.UI;

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
			menu.Item("Start", Start);
			menu.Item("Quit", Quit);
			menu.NotClosable();
			return menu;
		}
		#endregion

		#region Procedures
		private void Start()
		{
			Close();
			this.Log("Starting game");
			GodotEventSystem.Broadcast(new StartGameEvent());
		}

		private void Quit()
		{
			this.Log("Quitting game");
			GetTree().Quit();
		}
		#endregion
	}
}
