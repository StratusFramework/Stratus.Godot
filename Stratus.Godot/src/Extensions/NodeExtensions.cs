﻿using Godot;

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
			return node.GetChildrenOfType<TNode>().FirstOrDefault();
		}

		public static TNode? GetParentOfType<TNode>(this Node node)
			where TNode : Node
		{
			Node current = node.GetParent();
			while (current != null)
			{
				if (current is TNode result)
				{
					return result;
				}
				current = current.GetParent();
			}
			return null;
		}

		public static void Toggle(this Node node)
		{
			if (node.ProcessMode == Node.ProcessModeEnum.Inherit)
			{
				node.Toggle(false);
			}
			else if (node.ProcessMode == Node.ProcessModeEnum.Disabled)
			{
				node.Toggle(true);
			}
		}

		public static void Toggle(this Node node, bool enable)
		{
			node.ProcessMode = enable ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
			if (node is Node3D node3d)
			{
				node3d.Visible = enable;
			}
		}

		/// <summary>
		/// Removes all the children from this node
		/// </summary>
		/// <param name="node"></param>
		public static void ClearChildren(this Node node)
		{
			foreach (var child in node.GetChildren())
			{
				//node.RemoveChild(child);
				child.Destroy();
			}
		}

		public static IEnumerable<TNode> GetChildrenOfType<TNode>(this Node node)
			where TNode : Node
		{
			return node.GetChildren().OfType<TNode>();
		}

		public static void Log(this Node node, string message)
		{
			StratusLog.Info($"[{node.Name}] {message}");
		}

		public static void Log(this Node node, object obj) => Log(node, obj.ToString());

		public static void LogWarning(this Node node, string message)
		{
			StratusLog.Warning($"[{node.Name}] {message}");
		}

		public static void LogError(this Node node, string message)
		{
			StratusLog.Error($"[{node.Name}] {message}");
		}

		public static void Log(this Node node, Result result)
			=> StratusLog.Result(result);

		public static void Invoke(this Node node, Action action, double duration)
		{
			node.GetTree().CreateTimer(duration).Timeout += action;
		}

		public static TNode? InstantiateScene<TNode>(this Node parent, PackedScene scene)
			where TNode : Node
		{
			var instance = scene.Instantiate<TNode>();
			parent.AddChild(instance);
			return instance;
		}

		public static void Destroy(this Node instance)
		{
			instance.GetParent().RemoveChild(instance);		
			//instance.GetTree().CurrentScene.RemoveChild(instance);
			instance.QueueFree();
		}

		public static void Quit(this Node node, int exitCode = 0)
		{
			node.GetTree().Quit(exitCode);
		}
	}
}
