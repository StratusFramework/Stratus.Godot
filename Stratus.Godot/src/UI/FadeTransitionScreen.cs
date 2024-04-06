using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Interpolation;
using Stratus.Models.UI;

namespace Stratus.Godot.UI
{
	/// <summary>
	/// Handles doing a fade transition through the usage of a <see cref="ColorRect"/>
	/// </summary>
	public partial class FadeTransitionScreen : CanvasLayer, IFadeEventHandler
	{
		[Export]
		private ColorRect colorRect;
		[Export]
		private bool fadeOutOnStart = true;

		private ActionGroup actions = new();

		public override void _Ready()
		{
			base._Ready();
			EventSystem.Connect<FadeInEvent>(FadeIn);
			EventSystem.Connect<FadeOutEvent>(FadeOut);
			EventSystem.Connect<FadeOutInEvent>(FadeOutIn);

			if (fadeOutOnStart)
			{
				FadeOut(new FadeOutEvent(0, null));
			}
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			actions.Update((float)delta);
		}

		public void FadeOutIn(FadeOutInEvent e)
		{
			this.Log(e);
			var seq = actions.Sequence();
			seq.Property(() => colorRect.Color, colorRect.Color.Alpha(1f), e.fadeOutDuration, Ease.Linear);
			seq.Call(e.transition);
			seq.Property(() => colorRect.Color, colorRect.Color.Alpha(0f), e.fadeInDuration, Ease.Linear);
		}

		public void FadeIn(FadeInEvent e)
		{
			this.Log(e);
			const float alpha = 0f;
			if (e.duration == 0f)
			{
				colorRect.Color = colorRect.Color.Alpha(alpha);
				e.onFinished?.Invoke();
			}
			else
			{
				var seq = actions.Sequence();
				seq.Property(() => colorRect.Color, colorRect.Color.Alpha(alpha), e.duration, Ease.Linear);
				if (e.onFinished != null)
				{
					seq.Call(e.onFinished);
				}
			}
		}

		public void FadeOut(FadeOutEvent e)
		{
			this.Log(e);
			const float alpha = 1f;
			if (e.duration == 0f)
			{
				colorRect.Color = colorRect.Color.Alpha(alpha);
				e.onFinished?.Invoke();
			}
			else
			{
				var seq = actions.Sequence();
				seq.Property(() => colorRect.Color, colorRect.Color.Alpha(alpha), e.duration, Ease.Linear);
				if (e.onFinished != null)
				{
					seq.Call(e.onFinished);
				}
			}
		}
	}
}
