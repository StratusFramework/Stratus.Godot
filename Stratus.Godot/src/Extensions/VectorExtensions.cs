using Godot;

using Stratus.Numerics;

namespace Stratus.Godot.Extensions
{
	public static class VectorExtensions
	{
		public static Vector2I ToVector2I(this Vector2Int value)
			=> new Vector2I(value.x, value.y);

		public static Vector3Int ToVector3Int(this Vector2I value) => new Vector3Int(value.X, value.Y);

		public static Vector2Int ToVector2Int(this Vector2I value) => new Vector2Int(value.X, value.Y);

		public static System.Numerics.Vector3 ToSystemVector3(this Vector3 value) => new System.Numerics.Vector3(value.X, value.Y, value.Z);


		public static Vector2 ToVector2(this Vector3 input) => new Vector2(input.X, input.Y);
	}
}
