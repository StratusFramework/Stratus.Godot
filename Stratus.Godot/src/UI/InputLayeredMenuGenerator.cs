using Godot;

using Stratus.Collections;
using Stratus.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Logging;
using Stratus.Models.UI;
using Stratus.Utilities;

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
		public Node root { get; }
		private bool visible
		{
			set
			{
				if (root is CanvasLayer cl)
				{
					cl.Visible = value;
				}
				else if (root is Control c)
				{
					c.Visible = value;
				}
			}
		}
		public bool opened { get; private set; }
		private Container container { get; }

		public event Action onOpen;
		public event Action onClose;

		public InputLayeredMenuGenerator(Node root, Container container)
		{
			input = new InputLayerButtonNavigator(root.Name);
			input.onCancel += () =>
			{
				if (current.exitOnCancel)
				{
					Close();
				}
			};
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
			opened = visible = true;
			input.layer.Push();
		}

		private void Open(Menu menu, bool focus)
		{
			Clear();
			current = menu;

			List<Button> buttons = new List<Button>();
			foreach (var entry in menu.items)
			{
				Button button = new Button();				
				button.Text = entry.name;
				Action action = default;

				if (entry is MenuItem item)
				{
					action = () =>
					{
						if (item.action())
						{
							Close();
						}
					};
					container.AddChild(button);
				}
				// Unlike others, this will require a label + control
				else if (entry is MenuOption option)
				{
					FlowContainer frame = new FlowContainer();					
					container.AddChild(frame);
					frame.AddChild(button);

					switch (option.reference.inferredType)
					{
						case Reflection.InferredType.Boolean:
							CheckButton checkBox = new CheckButton();
							checkBox.ToggleMode = (bool)option.reference.value;
							checkBox.Toggled += value =>
							{
								option.reference.value = value;
							};
							frame.AddChild(checkBox);
							break;

						case Reflection.InferredType.Integer:
							break;
						case Reflection.InferredType.Float:
							break;
						case Reflection.InferredType.String:
							break;
						case Reflection.InferredType.Enum:
							OptionButton optionButton = new OptionButton();
							var values = EnumUtility.Values(option.reference.type);
							values.ForEach(v => optionButton.AddItem(v.ToString()));
							optionButton.Selected = EnumUtility.ToInteger((Enum)option.reference.value);
							optionButton.ItemSelected += idx =>
							{
								var value = values[idx];
								option.reference.value = value;
							};
							frame.AddChild(optionButton);
							break;

						case Reflection.InferredType.Vector2:
							break;
						case Reflection.InferredType.Vector3:
							break;
						case Reflection.InferredType.Vector4:
							break;
						case Reflection.InferredType.Color:
							break;
					}
				}
				else if (entry is Menu subMenu)
				{
					action = () =>
					{
						Open(subMenu, true);
					};
					container.AddChild(button);
				}

				if (action == null)
				{
					action = () => { };
				}

				button.Pressed += action;
				buttons.Add(button);
			}

			input.Set(buttons.ToArray());
			if (focus)
			{
				input.Focus();
			}
		}

		public override void Close()
		{
			// Close for good
			if (current == null
				|| (current.parent == null))
			{
				opened = visible = false;
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
