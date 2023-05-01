using Godot;

using Stratus.Collections;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Logging;
using Stratus.Models.UI;

using System;
using System.Collections.Generic;

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
			layer = new MenuInputLayer(name);
			layer.cancel = Cancel;
		}

		public void Set(params Button[] values)
		{
			Clear();
			buttons.AddRange(values);
			menuNavigator.Set(buttons);
		}

		public void Clear()
		{
			buttons.Clear();
			menuNavigator.Clear();
		}

		public void Focus()
		{
			if (menuNavigator.valid)
			{
				menuNavigator.current?.GrabFocus();
			}
		}

		protected virtual void Cancel()
		{
			onCancel?.Invoke();
		}
	}

	public class MenuGeneratorArguments
	{
		public Control control { get; }

		public MenuGeneratorArguments(Control control)
		{
			this.control = control;
		}
	}

	public class InputLayeredMenuGenerator : MenuGenerator
	{
		private record Instance(IMenuEntry entry, Node node);

		private InputLayerButtonNavigator input { get; }
		public Control root { get; }
		private Container container { get; }

		public event Action onOpen;
		public event Action onClose;

		public InputLayeredMenuGenerator(Control root, Container container)
		{
			input = new InputLayerButtonNavigator(root.Name);
			input.onCancel += Close;
			input.layer.onActive += active =>
			{
				//root.Visible = true;
				if (active)
				{
					input.Focus();
				}
			};
			this.root = root;
			this.container = container;
		}

		public override void Open(Menu menu)
		{
			Open(menu, false);
			onOpen?.Invoke();
			root.Visible = true;
			input.layer.Push();
		}

		private void Open(Menu menu, bool focus)
		{
			Clear();
			current = menu;

			List<Button> buttons = new List<Button>();
			foreach (var item in menu.items)
			{
				Button button = new Button();
				button.Text = item.name;
				Action action = default;

				if (item is MenuItem menuItem)
				{
					action = () =>
					{
						if (menuItem.action())
						{
							Close();
						}
					};
				}
				else if (item is Menu subMenu)
				{
					action = () =>
					{
						Open(subMenu, true);
					};
				}

				button.Pressed += action;
				buttons.Add(button);
				container.AddChild(button);
			}

			input.Set(buttons.ToArray());
			if (focus)
			{
				input.Focus();
			}
		}

		public override void Close()
		{
			if (current == null || current.parent == null)
			{
				root.Visible = false;
				onClose?.Invoke();
				input.layer.Pop();
				input.Clear();
				Clear();
			}
			else
			{
				Open(current.parent, true);
			}
		}

		private void Clear()
		{
			foreach (var child in container.GetChildren())
			{
				child.QueueFree();
			}
		}
	}
}
