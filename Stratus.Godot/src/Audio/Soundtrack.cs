using Godot;
using System;

namespace Stratus.Godot.Audio
{
	[GlobalClass]
	public partial class Soundtrack : Resource
	{
		[Export]
		private global::Godot.Collections.Dictionary<string, AudioStream> tracks = new();

		public Soundtrack()
		{
		}

		public bool TryGet(string name, out AudioStream stream)
		{
			stream = null;
			if (tracks.ContainsKey(name))
			{
				stream = tracks[name];
				return true;
			}
			return false;
		}

		public bool TryGet(Enum name, out AudioStream stream) 
			=> TryGet(name.ToString(), out stream);
	}
}
