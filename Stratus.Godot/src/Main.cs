using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Models.Gameflow;
using Stratus.Models.States;
using Stratus.Models.UI;

namespace Stratus.Godot
{
	/// <summary>
	/// The root node of the simulation
	/// </summary>
	public partial class Main : Node
	{
		[Export]
		public PackedScene scene;

		private Node gameNode;

		public override void _Ready()
		{
			EventSystem.configuration.logAll = true;
			GameState.Changed(OnGameStateChanged);
			EventSystem.Connect<StartGameEvent>(OnGameStartedEvent);
			EventSystem.Connect<EndGameEvent>(OnGameEndedEvent);
			EventSystem.Broadcast(new FadeOutEvent(0f, OpenMainMenu));			
		}

		private void OnGameStateChanged(State state, StateTransition action)
		{
			this.Log($"GAMESTATE {action.ToString().ToUpper()} {state.name}");
		}

		private void OnGameStartedEvent(StartGameEvent e)
		{
			StratusLog.AssertNotNull(scene, "Scene not set");
			gameNode = this.InstantiateScene<Node>(scene);
			this.Log($"Starting game with scene {gameNode.Name}");
		}

		private void OnGameEndedEvent(EndGameEvent e)
		{
			GameState.Return<MainMenuState>();
			gameNode.Destroy();
			this.Log("Ended game");
		}

		private void OpenMainMenu()
		{
			GameState.Enter<MainMenuState>();
			EventSystem.Broadcast(new FadeInEvent(1f, null));
		}
	}

	public class DefaultLogger : GodotLogger
	{
	}
}
