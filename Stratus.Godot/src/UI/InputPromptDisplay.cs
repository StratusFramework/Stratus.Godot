using Godot;
using Godot.Collections;

using Stratus.Events;
using Stratus.Extensions;
using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Inputs;

using System.Collections.Generic;
using System.Linq;

namespace Prototypes.Stratus.Godot.UI
{
	public partial class InputPromptDisplay : CanvasLayer
	{
		[Export]
		private Container container;
		[Export]
		private PackedScene entryScene;
		[Export]
		private InputPromptMap inputPrompts;

		private Array<StringName> actions;
		private global::Godot.Collections.Dictionary<StringName, Array<InputEvent>> actionEvents;
		private global::Godot.Collections.Dictionary<string, Container> prompts = new();
		private int deviceIndex;

		public override void _Ready()
		{
			base._Ready();

			actions = InputMap.GetActions();
			actionEvents = new();
			foreach (var action in actions)
			{
				var events = InputMap.ActionGetEvents(action);
				actionEvents.Add(action, events);
			}

			EventSystem.Connect<ToggleInputPromptEvent>(e =>
			{
				Toggle(e);
			});
		}

		public override void _Input(InputEvent e)
		{
			base._Input(e);
			if (deviceIndex != e.Device)
			{
				deviceIndex = e.Device;
				UpdateEntries();
			}
		}

		public void Clear()
		{
			container.ClearChildren();
			prompts.Clear();
		}

		private void UpdateEntries()
		{
			foreach (var node in container.GetChildren())
			{
				// TODO: Map the icon
			}
		}

		private void Toggle(ToggleInputPromptEvent e)
		{
			bool exists = prompts.ContainsKey(e.prompt.action);
			if (e.toggle)
			{
				if (!exists)
				{
					var instance = entryScene.Instantiate<Container>();
					SetPrompt(instance, e);
					prompts.Add(e.prompt.action, instance);
					container.AddChild(instance);
				}
			}
			else
			{
				if (exists)
				{
					var instance = prompts[e.prompt.action];
					prompts.Remove(e.prompt.action);
					instance.Destroy();
				}
			}
		}

		private IEnumerable<Texture2D> GetActionIcons(string action)
		{
			// TODO: Resolve the device
			//var events = actionEvents[action];
			//var inputEvent = events.FirstOrDefault(e => e.Device == deviceIndex);
			//if (inputEvent != null)
			//{
			//	var name = inputEvent.AsText();
			//	this.Log($"Will use icon {name}");
			//}
			//else
			//{
			//	this.Log($"No input event for action {action} for device {deviceIndex}. Devices: {events.Select(e => e.Device).ToStringJoin().Enclose(StratusStringEnclosure.Parenthesis)}");
			//}

			if (inputPrompts.actionIcons.TryGetValue(action, out var icons))
			{
				return icons.Cast<Texture2D>();
			}

			this.LogWarning($"No action icons found for {action}");

			return null;
		}

		private void SetPrompt(Container container, ToggleInputPromptEvent e)
		{
			var richText = container.GetChildOfType<RichTextLabel>();
			richText.Text = e.prompt.message;

			var icons = GetActionIcons(e.prompt.action);
			if (icons != null) 
			{
				var iconsContainer = container.FindChild("Icons");
				foreach(var icon in icons)
				{
					var textureRect = new TextureRect();
					textureRect.Texture = icon;
					textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;

					iconsContainer.AddChild(textureRect);
				}
			}
			// TODO: Map the input action to the right icons
		}
	}
}
