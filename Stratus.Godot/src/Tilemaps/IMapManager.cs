using Godot;

using Stratus.Models.Maps;

namespace Stratus.Godot.TileMaps
{
	public interface IMapManager
	{
		TileMap? tileMap { get; }
		Vector2I cursorPosition { get; }
		IMap2D map { get; }
	}

	// TODO: Refactor between setup map movement and encounter map movement
	public abstract partial class MapManager : Node2D, IMapManager
	{
		public abstract TileMap? tileMap { get; }
		public abstract Vector2I cursorPosition { get; }
		public abstract IMap2D map { get; }

		public record LoadEvent : TileMapEvent
		{
			public LoadEvent(TileMap tileMap) : base(tileMap)
			{
			}
		}

		public override string ToString() => Name;
	}
}
