using Godot;

using Stratus.Godot.Extensions;
using Stratus.Godot.TileMaps;
using Stratus.Models.Maps;
using Stratus.Numerics;
using Stratus.Search;
using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratus.Godot.Tilemaps
{
	public class GodotMap : Map2D<TileMap, TileMapNode, TileData>
	{
		public GodotMap(TileMap tileMap) : base(tileMap)
		{
		}

		public override void Initialize()
		{
			var rect = tileMap.GetUsedRect();
			var size = new Vector2Int(rect.Size.X, rect.Size.Y);
			_grid = new Grid2D<CellReference<TileMapNode, TileData>, DefaultMapLayer>(size, CellLayout.Rectangle);

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
					var result = _grid.Set(layer, new TileInfo(data, _pos), _pos);
					if (!result)
					{
						StratusLog.Result(result);
					}
				}
			}
		}
	}

	public class TileInfo : CellReference<TileMapNode, TileData>
	{
		public TileInfo(TileMapNode node) : base(node)
		{
		}

		public TileInfo(TileData data, Vector2Int position)
			: base(data, position)
		{
		}
	}

	public class GodotTileMapReference : TileMapReference<TileMap>
	{
		public GodotTileMapReference(TileMap tileMap) : base(tileMap)
		{
		}

		public override Vector2Int size
		{
			get
			{
				var rect = tileMap.GetUsedRect();
				return new Vector2Int(rect.Size.X, rect.Size.Y);
			}
		}

		public override int layerCount => tileMap.GetLayersCount();
	}
}
