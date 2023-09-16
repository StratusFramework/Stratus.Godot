using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.Audio;

namespace Stratus.Godot.Audio
{
	public partial class SoundtrackPlayer : AudioStreamPlayer
	{
		[Export]
		public AudioChannelType channel = AudioChannelType.Background;
		[Export]
		public Soundtrack soundtrack;

		public override void _Ready()
		{
			base._Ready();
			GodotEventSystem.Connect<PlayAudioEvent>(OnPlayAudioEvent);
			GodotEventSystem.Connect<StopAudioEvent>(OnStopAudioEvent);
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
			=> GodotEventSystem.Broadcast(e);

		public static void Stop(StopAudioEvent e)
			=> GodotEventSystem.Broadcast(e);
	}
}
