using Godot;

using Stratus.Events;
using Stratus.Inputs;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Stratus.Godot
{
	public class CompositeActionNames : IEnumerable<StringName>
	{
		public StringName xPositive;
		public StringName xNegative;

		public StringName yPositive;
		public StringName yNegative;

		public IEnumerator<StringName> GetEnumerator()
		{
			yield return xPositive;
			yield return xNegative;
			yield return yPositive;
			yield return yNegative;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}

	public class GodotInputActionMapHandler : ActionMapHandler<InputEvent>
	{
		private string _name;
		public override string name => _name;

		/// <summary>
		/// The pattern when checking <see cref="Vector2"/> values,
		/// which consists of 4 suffixes that check -X, +X, -Y, +Y
		/// </summary>
		public string[] vector2Pattern { get; } = new string[]
		{
			"_left",
			"_right",
			"_down",
			"_up"
		};

		private Dictionary<string, CompositeActionNames> compositeActionNames
			= new Dictionary<string, CompositeActionNames>();


		public GodotInputActionMapHandler()
		{
		}

		public GodotInputActionMapHandler(string name, bool lowercase = true)
		{
			this._name = name;
			this.lowercase = lowercase;
		}

		public override bool HandleInput(InputEvent input)
		{
			bool handled = false;
			if (input.IsActionType())
			{
				foreach (var binding in actions.Values)
				{
					switch (binding.type)
					{
						case InputActionType.Button:
							{
								if (input.IsAction(binding.name))
								{
									binding.action(input);
									handled = true;
								}
							}
							break;

						case InputActionType.Composite:
							{
								binding.action(input);
								handled = true;
							}
							break;
					}
				}
			}

			return handled;
		}

		public void Bind<TAction>(TAction name, Action<Vector2> action)
			where TAction : Enum
		{
			Bind(name.ToString(), action);
		}

		public void Bind(string name, Action<Vector2> action)
		{
			name = name.ToLowerInvariant();

			compositeActionNames.Add(name, new CompositeActionNames()
			{
				xNegative = $"{name}{vector2Pattern[0]}",
				xPositive = $"{name}{vector2Pattern[1]}",
				yPositive = $"{name}{vector2Pattern[2]}",
				yNegative = $"{name}{vector2Pattern[3]}"
			});

			Bind(name, InputActionType.Composite, inputEvent =>
			{
				var actionNames = compositeActionNames[name];

				if (inputEvent.IsAction(actionNames.xNegative) || inputEvent.IsAction(actionNames.xPositive)
					|| inputEvent.IsAction(actionNames.yPositive) || inputEvent.IsAction(actionNames.yNegative))
				{
					var value = Input.GetVector(actionNames.xNegative, actionNames.xPositive, actionNames.yPositive, actionNames.yNegative);
					action(value);
				}
			});
		}

		public void BindPress(string action, Action<bool> onToggle)
		{
			Bind(action, InputActionType.Button, e =>
			{
				if (e.IsActionPressed(action))
				{
					onToggle?.Invoke(true);
				}
				else if (e.IsActionReleased(action))
				{
					onToggle?.Invoke(false);
				}
			});
		}

		public void Bind(string name, Action<Vector2I> action)
		{
			Bind(name, (Vector2 value) => action(new Vector2I((int)value.X, -(int)value.Y)));
		}

		public override bool TryBind(string name, object deleg)
		{
			if (deleg is Action action)
			{
				Bind(name, action);
				return true;
			}
			else if (deleg is Action<Vector2> vec2Action)
			{
				Bind(name, vec2Action);
			}
			return false;
		}
	}

}
