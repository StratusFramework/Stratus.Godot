using Godot;

using Prototypes;

using Stratus.Godot.Extensions;
using Stratus.Models.Gameflow;
using Stratus.Models.States;
using Stratus.src.Models.Games;

namespace Stratus.Godot
{
    public partial class Main : Node
	{
		[Export]
		public PackedScene scene;

		private Node gameNode;

		public override void _Ready()
		{
			GodotEventSystem.Connect<StartGameEvent>(OnGameStartedEvent);
			GodotEventSystem.Connect<EndGameEvent>(OnGameEndedEvent);

			GameState.Changed(OnGameStateChanged);
			GameState.Enter<MainMenuState>();
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
	}

	public class DefaultLogger : GodotLogger
	{
	}
}
