using Godot;

using Stratus.Events;

using System;

namespace Stratus.Godot
{
	public static class EventSystemExtensions
	{
		public static void Broadcast<TEvent>(this Node node, TEvent e)
			where TEvent : Event
		{
			EventSystem.Broadcast(e);
		}

		public static void Connect<TEvent>(this Node node, Action<TEvent> onEvent)
			where TEvent : Event
		{
			EventSystem.Connect(node, onEvent);
		}

		public static void Subscribe<TEvent>(this Node node, Action<TEvent> onEvent)
			where TEvent : Event
		{
			EventSystem.Connect(onEvent);
		}
	}
}