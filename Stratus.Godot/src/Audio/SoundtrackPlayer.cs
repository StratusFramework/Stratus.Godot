using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Models.Audio;

namespace Stratus.Godot.Audio
{
	public partial class SoundtrackPlayer : AudioStreamPlayer
	{
		[Export]
		public DefaultAudioChannel channel = DefaultAudioChannel.Background;
		[Export]
		public Soundtrack soundtrack;

		public override void _Ready()
		{
			base._Ready();
			EventSystem.Connect<PlayAudioEvent>(OnPlayAudioEvent);
			EventSystem.Connect<StopAudioEvent>(OnStopAudioEvent);
		}

		private void OnStopAudioEvent(StopAudioEvent e)
		{
			if (e.channel != channel)
			{
				return;
			}

			this.Stop();
			this.Stream = null;
		}

		private void OnPlayAudioEvent(PlayAudioEvent e)
		{
			if (e.channel != channel)
			{
				return;
			}

			if (soundtrack.TryGet(e.name, out var stream))
			{
				if (stream != null)
				{
					this.Log($"Will play {e.name}");
					this.Stream = stream;
					this.Play();
				}
			}
			else
			{
				this.LogError($"Track {e.name} not found!");
			}
		}

		public static void Play(PlayAudioEvent e)
			=> EventSystem.Broadcast(e);

		public static void Stop(StopAudioEvent e)
			=> EventSystem.Broadcast(e);
	}
}
