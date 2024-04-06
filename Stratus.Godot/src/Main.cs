using Godot;
using Godot.Collections;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Godot.UI;
using Stratus.Inputs;
using Stratus.Interpolation;
using Stratus.Models.Gameflow;
using Stratus.Models.States;
using Stratus.Models.UI;

using System;

namespace Stratus.Godot
{
	/// <summary>
	/// The root node of the simulation
	/// </summary>
	public partial class Main : Node
	{
		[Export]
		public PackedScene scene;
		[Export]
		private float transitionDuration = 1f;

		private Node gameNode;
		private ActionGroup actions = new();

		public override void _Ready()
		{
			EventSystem.configuration.logAll = true;
			GameState.Changed(OnGameStateChanged);
			EventSystem.Connect<StartGameEvent>(OnGameStartedEvent);
			EventSystem.Connect<EndGameEvent>(OnGameEndedEvent);
			Transition<MainMenuState>(0f, 1f);
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			actions.Update((float)delta);
		}

		private void OnGameStateChanged(State state, StateTransition action)
		{
			this.Log($"GAMESTATE {action.ToString().ToUpper()} {state.name}");
		}

		private void OnGameStartedEvent(StartGameEvent e)
		{
			StratusLog.AssertNotNull(scene, "Scene not set");
			Transition(transitionDuration, () => gameNode = this.InstantiateScene<Node>(scene), transitionDuration);
		}

		private void OnGameEndedEvent(EndGameEvent e)
		{
			Transition(transitionDuration, () =>
			{
				GameState.Return<MainMenuState>();
				gameNode.Destroy();
			}, transitionDuration);
			this.Log("Ended game");
		}

		private void Transition(float fadeOut, Action action, float fadeIn)
		{
			actions.Sequence()
				.Event(new ToggleInputEvent(false))
				.Event(new FadeOutEvent(fadeOut))
				.Delay(fadeOut)
				.Call(action)
				.Event(new FadeInEvent(fadeIn))
				.Delay(fadeIn)
				.Event(new ToggleInputEvent(true));
		}

		private void OpenMainMenu()
		{
			GameState.Enter<MainMenuState>();
			EventSystem.Broadcast(new FadeInEvent(1f, null));
		}

		private void Transition<TState>(float fadeOutDuration, float fadeInDuration)
			where TState : State
		{
			EventSystem.Broadcast(new FadeOutInEvent(fadeOutDuration, () => GameState.Enter<TState>(), fadeInDuration));
		}
	}

	public class DefaultLogger : GodotLogger
	{
	}
}
