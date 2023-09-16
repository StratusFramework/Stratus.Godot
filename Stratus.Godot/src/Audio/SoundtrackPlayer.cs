using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.Audio;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratus.Godot.Audio
{
	public partial class SoundtrackPlayer : AudioStreamPlayer
	{
		public override void _Ready()
		{
			base._Ready();
			GodotEventSystem.Connect<PlayAudioEvent>(OnPlayAudioEvent);
		}

		private void OnPlayAudioEvent(PlayAudioEvent e)
		{
			this.Log($"Will play {e.name}");
		}
	}
}
