using Godot;

using Stratus.Collections;
using Stratus.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Inputs;
using Stratus.Logging;
using Stratus.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Godot.UI
{
	public class InputLayerButtonNavigator : IStratusLogger
	{
		public MenuInputLayer layer { get; } 
		protected List<Button> buttons = new();
		protected ArrayNavigator<Button> menuNavigator = new();

		public event Action onCancel;

		public InputLayerButtonNavigator(string name)
		{
			menuNavigator.onChanged += (curr, prev) => curr.GrabFocus();
			//layer = new(name);
			layer = new MenuInputLayer(name);
			//layer.move = (dir) =>
			//{
			//	this.Log($"Moved {dir}");
			//	menuNavigator.Navigate(new System.Numerics.Vector2(dir.X, dir.Y));
			//};
			//layer.select = () => menuNavigator.current._Pressed();
			layer.cancel = Cancel;
		}
		
		public void Set(params Button[] values)
		{
			buttons.Clear();
			buttons.AddRange(values);
			menuNavigator.Set(buttons);
		}

		public void Focus()
		{
			menuNavigator.current.GrabFocus();
		}

		protected virtual void Cancel()
		{
			onCancel?.Invoke();
		}
	}

	public class InputLayeredMenuGenerator : InputLayerButtonNavigator
	{
		public Container container { get; }
		public Control root { get; }

		public event Action onOpen;
		public event Action onClose;

		public InputLayeredMenuGenerator(Container container, Control root = null)
			: base(root.Name)
		{
			this.container = container;
			this.root = root ?? container;
		}

		public void Open(params LabeledAction[] actions)
		{
			this.Log($"Opening Menu at {root}");

			var actionButtons = actions.Select(action =>
			{
				var button = new Button();
				button.Text = action.label;
				button.Pressed += () =>
				{
					Close();
					action.action();
				};
				container.AddChild(button);
				return button;
			}).ToArray();

			Set(actionButtons);
			root.Visible = true;
			Focus();

			layer.Push();
			onOpen?.Invoke();
		}

		protected override void Cancel()
		{
			base.Cancel();
			Close();
		}

		public void Close()
		{
			this.Log($"Closing Menu at {root}");

			root.Visible = false;
			onClose?.Invoke();
			layer.Pop();

			foreach (var child in container.GetChildren())
			{
				child.QueueFree();
			}
		}
	}
}
