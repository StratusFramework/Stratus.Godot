using Godot;

using Stratus.Logging;

using System;

namespace Stratus.Godot
{
	/// <summary>
	/// The logger to use for Godot.
	/// </summary>
	/// <remarks>To use, make sure to subclass from it in your main project.</remarks>
	public abstract class GodotLogger : StratusLogger
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
