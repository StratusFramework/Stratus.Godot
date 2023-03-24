using Godot;

using Stratus.Inputs;
using Stratus.Godot.Extensions;
using Stratus.Extensions;

namespace Stratus.Godot.Inputs
{
	public abstract class GodotPlayerInput : Node
	{
		#region Properties
		private InputStack<InputLayer> inputLayers = new InputStack<InputLayer>();
		/// <summary>
		/// The current input layer
		/// </summary>
		public InputLayer layer => inputLayers.current;
		public bool hasInputLayer => inputLayers.hasLayers;
		/// <summary>
		/// Can be used to toggle input processing
		/// </summary>
		public bool inputEnabled { get; set; } = true;

		public double transitionDuration { get; set; } = 0.1;
		#endregion

		#region Messages
		public override void _Ready()
		{
			GodotEventSystem.Connect<InputLayer.PushEvent>(OnPushLayerEvent);
			GodotEventSystem.Connect<InputLayer.PopEvent>(OnPopLayerEvent);
			inputLayers.onLayerToggled += this.OnInputLayerChanged;
			inputLayers.onPush += layer => this.LogInfo($"Input layer <{layer}> was pushed ({inputLayers.count})");
			inputLayers.onPop += layer => this.LogInfo($"Input layer <{layer}> was popped ({inputLayers.count})");
			inputLayers.onQueue += layer => this.LogInfo($"Input layer <{layer}> was queued ({inputLayers.count})");
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (inputEnabled && hasInputLayer)
			{
				layer.HandleInput(@event);
			}
		}
		#endregion

		#region Event Handlers
		private void OnPushLayerEvent(InputLayer.PushEvent e)
		{
			var push = inputLayers.Push(e.layer);
			if (push)
			{
				StratusLog.Info($"Pushed input layer <{e.layer}>");
			}
		}

		private void OnPopLayerEvent(InputLayer.PopEvent e)
		{
			inputEnabled = false;
			inputLayers.Pop();
			this.Invoke(() => inputEnabled = true, transitionDuration);
		}

		private void OnInputLayerChanged(InputLayer layer)
		{
			this.LogInfo($"Input layer <{layer}> is now {(layer.active ? "active" : "inactive")}");
		}
		#endregion

	}
}
