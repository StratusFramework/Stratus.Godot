using Godot;

using System;

namespace Stratus.Godot.Actors
{
	public partial class StateMachineRig3D : Node3D
	{
		[Export]
		public AnimationTree animationTree;
		[Export]
		public bool idleAtStart = true;

		private AnimationNodeStateMachinePlayback stateMachine;

		public override void _Ready()
		{
			base._Ready();
			stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
			SetState(DefaultAnimation.Idle);
		}

		public void SetState(Enum state)
		{
			stateMachine.Travel(state.ToString());
		}

		public void Set(DefaultAnimation state) => SetState(state);
	}

	public enum DefaultAnimation
	{
		Idle,
		Walk,
		Run,

		Interact,
		Jump
	}
}