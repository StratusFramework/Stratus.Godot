﻿using Godot;

using Stratus.Numerics;

namespace Stratus.Godot.Extensions
{
	public static class VectorExtensions
	{
		public static Vector2I ToVector2I(this Vector2Int value)
			=> new Vector2I(value.x, value.y);

		public static Vector3Int ToVector3Int(this Vector2I value)
		{
			return new Vector3Int(value.X, value.Y);
		}
	}
}
