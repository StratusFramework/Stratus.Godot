using Godot;

using Stratus.Inputs;
using Stratus.Godot.Extensions;

namespace Stratus.Godot.Inputs
{
	public abstract class GodotPlayerInput : Node
	{
		#region Properties
		private InputStack<InputLayer> inputLayers = new InputStack<InputLayer>();
		/// <summary>
		/// The current input layer
		/// </summary>
		public InputLayer layer => inputLayers.activeLayer;
		public bool hasInputLayer => inputLayers.hasActiveLayers;
		/// <summary>
		/// Can be used to toggle input processing
		/// </summary>
		public bool inputEnabled { get; set; } = true;
		#endregion

		#region Messages
		public override void _Ready()
		{
			GodotEventSystem.Connect<InputLayer.PushEvent>(OnPushLayerEvent);
			GodotEventSystem.Connect<InputLayer.PopEvent>(OnPopLayerEvent);
			inputLayers.onInputLayerChanged += this.OnInputLayerChanged;
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
			StratusLog.Info($"Pushing input layer {e.layer}");
			inputLayers.Push(e.layer);
		}

		private void OnPopLayerEvent(InputLayer.PopEvent e)
		{
			// Pop layers that have been marked as inactive
			while (inputLayers.canPop)
			{
				StratusLog.Info($"Popping input layers");
				inputLayers.Pop();
			}
		}

		private void OnInputLayerChanged(InputLayer layer)
		{
			StratusLog.Info($"Input layer now {layer}");
		}
		#endregion

	}
}
