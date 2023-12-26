using Godot;

using Prototypes;

using Stratus.Godot.Extensions;
using Stratus.Models.States;

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

			StateStack.Changed(OnGameStateChanged);
			StateStack.Enter<MainMenuState>();
		}

		private void OnGameStateChanged(GameState state, StateTransition action)
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
			StateStack.Return<MainMenuState>();
			gameNode.Destroy();
			this.Log("Ended game");
		}
	}

	public class DefaultLogger : GodotLogger
	{
	}
}
