using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.States;

namespace Stratus.Godot
{
	public partial class Main : Node
	{
		[Export]
		public PackedScene scene;

		public override void _Ready()
		{
			GodotEventSystem.Connect<NewGameEvent>(OnGameStartedEvent);
			GameState.onChange += this.OnGameStateChanged;
			GameState.Push<MainMenuState>();
		}

		private void OnGameStateChanged(GameState state, StateTransition transition)
		{
			this.Log($"Game state {state.name} is now {transition}");
		}

		private void OnGameStartedEvent(NewGameEvent e)
		{
			StratusLog.AssertNotNull(scene, "Scene not set");
			var node = this.InstantiateScene<Node>(scene);
			this.Log($"Starting game with scene {node.Name}");
		}
	}

	public class DefaultLogger : GodotLogger
	{
	}
}
