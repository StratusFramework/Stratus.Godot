using Godot;

using Stratus.Godot.TileMaps;
using Stratus.Godot.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Stratus.Godot.TileMaps
{
	public abstract partial class TileMapGizmo : Node2D
	{
		private MenuInputLayer input = new MenuInputLayer();

		public TileMap tileMap { get; private set; }

		protected abstract void OnReady();
		protected abstract void Move(Vector2I input);
		protected abstract void OnConfirm();
		protected abstract void OnCancel();

		public override void _Ready()
		{
			input.move += Move;
			input.select += Confirm;
			input.cancel += Cancel;

			this.Connect<MapManager.LoadEvent>(e =>
			{
				Initialize(e.tileMap);
			});

			OnReady();
		}

		public void Initialize(TileMap tileMap)
		{
			this.tileMap = tileMap;
		}

		private void Show()
		{
			input.Push();
			Visible = true;
		}

		private void Hide()
		{
			input.Pop();
			Visible = false;
		}

		public void Confirm()
		{
			Hide();
		}

		public void Cancel()
		{
			OnCancel();
			Hide();
		}
	}
}
