using Godot;

using System;

namespace Stratus.Godot.UI
{
	public partial class FadeTransitionScreen : Node2D
	{
		[Export]
		private AnimationPlayer player;
		[Export]
		private string fadeInAnimation = "FadeIn";
		[Export]
		private string fadeOutAnimation = "FadeOut";

		private Action queuedAction;

		public override void _Ready()
		{
			base._Ready();
			player.AnimationFinished += this.OnAnimationFinished;
		}

		private void OnAnimationFinished(StringName animName)
		{
			if (queuedAction != null)
			{
				queuedAction();
			}
			queuedAction = null;
		}

		public async void FadeIn(Action action, bool instant = false)
		{
			queuedAction = action;
			player.Play(fadeInAnimation);
		}

		public async void FadeOut(Action action, bool instant = false)
		{
			queuedAction = action;
			player.Play(fadeOutAnimation);
		}
	}
}
