using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.States;

namespace Stratus.Godot
{
	public partial class Main : Node
	{
		[Export]
		public PackedScene scene;

		public override void _Ready()
		{
			GodotEventSystem.Connect<NewGameEvent>(OnGameStartedEvent);
		}

		private void OnGameStartedEvent(NewGameEvent e)
		{
			StratusLog.AssertNotNull(scene, "Scene not set");
			var node = this.InstantiateScene<Node>(scene);
			this.Log($"Starting game with scene {node.Name}");
		}
	}

	public class DefaultLogger : GodotLogger
	{
	}
}
