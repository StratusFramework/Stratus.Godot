using Godot;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Godot.Extensions
{
	public static class NodeExtensions
	{
		public static TNode? GetChildOfType<TNode>(this Node node)
			where TNode : Node
		{
			return node.GetChildren().OfType<TNode>().FirstOrDefault();
		}

		public static IEnumerable<TNode> GetChildrenOfType<TNode>(this Node node)
			where TNode : Node
		{
			return node.GetChildren().OfType<TNode>();
		}

		public static void LogInfo(this Node node, string message)
		{
			StratusLog.Info($"[{node.Name}] {message}");
		}

		public static void LogWarning(this Node node, string message)
		{
			StratusLog.Warning($"[{node.Name}] {message}");
		}

		public static void LogError(this Node node, string message)
		{
			StratusLog.Error($"[{node.Name}] {message}");
		}

		public static void Invoke(this Node node, Action action, double duration)
		{
			node.GetTree().CreateTimer(duration).Timeout += action;
		}
	}
}
