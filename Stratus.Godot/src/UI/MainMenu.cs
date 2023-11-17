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
	public partial class MainMenu : CanvasLayer
	{
		#region Fields
		[Export]
		public BoxContainer actionButtons;
		[Export]
		public Button startButton;
		[Export]
		public Button quitButton;

		private InputLayerButtonNavigator input = new InputLayerButtonNavigator("Main Menu");
		#endregion

		#region Messages
		public override void _Ready()
		{
			startButton.ButtonDown += Start;
			quitButton.ButtonDown += Quit;
			input.Set(startButton, quitButton);

			GameState.Set(new MainMenuState());
			GodotEventSystem.Connect<MainMenuEvent>(e => Open());
			GodotEventSystem.Connect<EndGameEvent>(e =>
			{
				Open();
			});

			Open();
		}
		#endregion

		#region Interface
		public void Add(params LabeledAction[] actions)
		{
			foreach(var action in actions)
			{
				Button button = new Button();
				button.Name = action.label;
				button.ButtonDown += action.action;

				actionButtons.AddChild(button);
			}
		}
		#endregion

		#region Callbacks
		public void Open()
		{
			Visible = true;
			startButton.GrabFocus();
			input.layer.Push();
			this.Invoke(() => SoundtrackPlayer.Play(new PlayAudioEvent(DefaultAudioChannel.Background, "mainmenu")), 1);
		}

		public void Close()
		{
			Visible = false;
			input.layer.Pop();
			SoundtrackPlayer.Stop(new StopAudioEvent(DefaultAudioChannel.Background));
		}
		#endregion

		#region Procedures
		private void Start()
		{
			Close();
			this.Log("Starting game");
			GodotEventSystem.Broadcast(new NewGameEvent());
		}

		private void Quit()
		{
			this.Log("Quitting game");
			GetTree().Quit();
		}
		#endregion
	}
}
