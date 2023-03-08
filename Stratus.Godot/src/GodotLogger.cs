using Godot;

using Stratus.Logging;

using System;

namespace Stratus.Godot
{
	public class GodotLogger : StratusLogger
	{
		public override void LogError(string message)
		{
			GD.PushError(message);
		}

		public override void LogException(Exception ex)
		{
			GD.PushError(ex.ToString());
		}

		public override void LogInfo(string message)
		{
			GD.Print(message);
		}

		public override void LogWarning(string message)
		{
			GD.PushWarning(message);
		}
	}
}
