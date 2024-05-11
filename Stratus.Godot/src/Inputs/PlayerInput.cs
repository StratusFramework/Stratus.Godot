using Godot;

using Stratus.Events;
using Stratus.Godot.Extensions;
using Stratus.Inputs;

using System;

namespace Stratus.Godot.Inputs
{
	public partial class PlayerInput : Node
	{
		#region Properties
		/// <summary>
		/// The current input layers
		/// </summary>
		private InputStack<InputLayer> inputLayers { get; } = new InputStack<InputLayer>();
		/// <summary>
		/// The current input layer
		/// </summary>
		public InputLayer layer => inputLayers.current;
		/// <summary>
		/// Whether an input layer is present
		/// </summary>
		public bool hasInputLayer => inputLayers.hasLayers;
		/// <summary>
		/// Can be used to toggle input processing
		/// </summary>
		public bool inputEnabled { get; set; } = true;

		public double transitionDuration { get; set; } = 0.1;
		public bool isKeyboard { get; private set; }
		#endregion

		#region Events
		public event Action<int> onInputDeviceChanged;
		#endregion

		#region Messages
		public override void _Ready()
		{
			EventSystem.Connect<InputLayer.PushEvent>(OnPushLayerEvent);
			EventSystem.Connect<InputLayer.PopEvent>(OnPopLayerEvent);
			EventSystem.Connect<ToggleInputEvent>(e =>
			{
				inputEnabled = e.toggle;
			});

			inputLayers.onPush += layer => this.Log($"PUSH <{layer}> ({inputLayers})");
			inputLayers.onPop += layer => this.Log($"POP <{layer}> ({inputLayers})");
			inputLayers.onQueue += layer => this.Log($"QUEUE <{layer}> ({inputLayers})");
		}

		public override void _UnhandledInput(InputEvent inputEvent)
		{
			// Keyboard/Mouse should be index 0
			OnInputDeviceUsed(inputEvent.Device);

			if (inputEnabled && hasInputLayer)
			{
				if (!layer.valid)
				{
					this.LogWarning($"Layer <{layer}> is not in a valid state");
				}
				else
				{
					layer.HandleInput(inputEvent);
					GetViewport().SetInputAsHandled();
				}
			}
		}
		#endregion

		#region Event Handlers
		private void OnPushLayerEvent(InputLayer.PushEvent e)
		{
			DisableInputTemporarily();
			inputLayers.Push(e.layer);
		}

		private void OnPopLayerEvent(InputLayer.PopEvent e)
		{
			DisableInputTemporarily();
			inputLayers.TryPop(e.layer);
		}

		private void OnInputDeviceUsed(int index)
		{
			bool isKeyboard = index == 0;
			if (this.isKeyboard == isKeyboard)
			{
				return;
			}

			this.isKeyboard = isKeyboard;
			onInputDeviceChanged?.Invoke(index);
		}

		private void DisableInputTemporarily()
		{
			inputEnabled = false;
			this.Invoke(() => inputEnabled = true, transitionDuration);

		}
		#endregion

		#region Static Helpers
		public static void ToggleInput(bool toggle) 
			=> EventSystem.Broadcast(new ToggleInputEvent(toggle));
		#endregion

	}
}
