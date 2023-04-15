using Godot;

using Stratus.Godot.Extensions;
using Stratus.Models.Maps;
using Stratus.Numerics;
using Stratus.Utilities;

namespace Stratus.Godot.Tilemaps
{
	public class GodotMap : Map2D<TileData>
	{
		public GodotMap(TileMap tileMap) : base(() => Generate(tileMap))
		{
		}

		private static Grid Generate(TileMap tileMap)
		{
			var rect = tileMap.GetUsedRect();
			var size = new Vector2Int(rect.Size.X, rect.Size.Y);
			var grid = new Grid(size, CellLayout.Rectangle);

			// Set the terrain, wall and object? layers
			var layerCount = tileMap.GetLayersCount();
			for (int l = 0; l < layerCount; l++)
			{
				string name = tileMap.GetLayerName(l);
				DefaultMapLayer layer = EnumUtility.Value<DefaultMapLayer>(name);
				var positions = tileMap.GetUsedCells(l);
				foreach (var pos in positions)
				{
					TileData data = tileMap.GetCellTileData(l, pos);
					var _pos = pos.ToVector2Int();
					var result = grid.Set(new Object2D(data.ToString(), layer, _pos), _pos);
					if (!result)
					{
						StratusLog.Result(result);
					}
				}
			}

			return grid;
		}
	}
}
