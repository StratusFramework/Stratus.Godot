using Godot;
using System.Collections.Generic;
using Stratus.Models;

using System;
using System.Linq;
using Stratus.Collections;
using Stratus.Logging;
using Stratus.Godot.Inputs;

namespace Stratus.Godot.UI
{
	public class InputLayeredMenuGenerator : IStratusLogger
	{
		private MenuInputLayer inputLayer = new MenuInputLayer();
		private List<Button> buttons = new();
		private ArrayNavigator<Button> menuNavigator;

		public Control root { get; }
		public Container container { get; }

		public event Action onOpen;
		public event Action onClose;

		public InputLayeredMenuGenerator(Container container, Control root = null)
		{
			this.container = container;
			this.root = root ?? container;

			menuNavigator = new ArrayNavigator<Button>();
			menuNavigator.onChanged += (curr, prev) => curr.GrabFocus();
			inputLayer.cancel = Close;
			inputLayer.select = () => menuNavigator.current._Pressed();
		}

		public void Open(params LabeledAction[] actions)
		{
			this.Log($"Opening {root}");

			buttons.Clear();

			foreach (var action in actions)
			{
				var button = new Button();
				button.Text = action.label;				
				button.Pressed += () =>
				{
					action.action();
					Close();
				};
				container.AddChild(button);
				buttons.Add(button);
			}

			menuNavigator.Set(buttons);
			menuNavigator.current.GrabFocus();
			root.Visible = true;

			inputLayer.Push();
			onOpen?.Invoke();
		}

		public void Close()
		{
			this.Log($"Closing {root}");
			root.Visible = false;
			onClose?.Invoke();

			inputLayer.Pop();
			foreach (var child in container.GetChildren())
			{
				child.QueueFree();
			}
		}

	}

	public enum MenuInputAction
	{
		Move,
		Select,
		Cancel,
	}

	public class MenuInputLayer : GodotInputLayer<MenuInputAction>
	{
		public Action<Vector2I> move;
		public Action select;
		public Action cancel;

		protected override void Initialize()
		{
			map.TryBindAll<MenuInputAction>();
		}
	}
}
