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
			//inputLayers.onLayerToggled += this.OnInputLayerChanged;
			//inputLayers.onPush += layer => this.LogInfo($"Input layer <{layer}> was pushed ({inputLayers.count})");
			//inputLayers.onPop += layer => this.LogInfo($"Input layer <{layer}> was popped ({inputLayers.count})");
			inputLayers.onQueue += layer => this.Log($"Input layer <{layer}> was queued ({inputLayers.count})");
			this.Log("Ready");
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (inputEnabled && hasInputLayer)
			{
				if (!layer.valid)
				{
					this.LogWarning($"Layer <{layer}> is not in a valid state");
				}
				else
				{
					layer.HandleInput(@event);
				}
			}
		}
		#endregion

		#region Event Handlers
		private void OnPushLayerEvent(InputLayer.PushEvent e)
		{
			DisableInputTemporarily();
			var push = inputLayers.Push(e.layer);
			StratusLog.Result(push);
		}

		private void OnPopLayerEvent(InputLayer.PopEvent e)
		{
			if (e.layer != layer)
			{
				this.LogWarning($"Could not pop layer <{e.layer}> as it not currently at the top (<{layer}>)");
				return;
			}

			DisableInputTemporarily();
			inputLayers.Pop();
		}

		//private void OnInputLayerChanged(InputLayer layer)
		//{
		//	this.LogInfo($"Input layer <{layer}> is now {(layer.active ? "active" : "inactive")}");
		//}

		private void DisableInputTemporarily()
		{
			inputEnabled = false;
			this.Invoke(() => inputEnabled = true, transitionDuration);

		}
		#endregion

	}
}
