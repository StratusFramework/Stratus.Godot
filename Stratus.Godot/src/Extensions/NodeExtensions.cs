using Godot;

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
	}
}
