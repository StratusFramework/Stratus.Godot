using Godot;
using Godot.Collections;

using System;

namespace Stratus.Godot.Inputs
{
	public partial class InputPromptMap : Resource
	{
		[Export]
		public Dictionary<StringName, Array<Resource>> actionIcons;
	}

}