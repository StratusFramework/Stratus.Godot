using Godot;

using Stratus.Events;

using System;

namespace Stratus.Godot
{
	public class GodotEventSystem : EventSystem<GodotObject>
	{
	}

	public static class EventSystemExtensions
	{
		public static void Broadcast<TEvent>(this Node node, TEvent e)
			where TEvent : Event
		{
			GodotEventSystem.Broadcast(e);
		}

		public static void Connect<TEvent>(this Node node, Action<TEvent> onEvent)
			where TEvent : Event
		{
			GodotEventSystem.Connect(onEvent);
		}
	}
}