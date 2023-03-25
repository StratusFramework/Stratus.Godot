using Godot;

using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Godot.UI;

namespace Stratus.Godot.TileMaps
{
	public abstract partial class TileMapGizmo : Node2D
	{
		private MenuInputLayer input;
		public TileMap tileMap { get; private set; }

		protected abstract void Move(Vector2I input);
		protected abstract void OnConfirm();
		protected abstract void OnCancel();

		public override void _Ready()
		{
			input = new MenuInputLayer(Name);

			input.move += Move;
			input.select += Confirm;
			input.cancel += Cancel;

			this.Connect<MapManager.LoadEvent>(e =>
			{
				Initialize(e.tileMap);
			});
		}

		public void Initialize(TileMap tileMap)
		{
			this.tileMap = tileMap;
		}

		protected void ShowGizmo()
		{
			this.LogInfo("Showing");
			input.Push();
			Visible = true;
		}

		protected void HideGizmo()
		{
			this.LogInfo("Hiding");
			input.Pop();
			Visible = false;
		}

		public void Confirm()
		{
			this.LogInfo("Confirm");
			OnConfirm();
			HideGizmo();
		}

		public void Cancel()
		{
			this.LogInfo("Cancel");
			OnCancel();
			HideGizmo();
		}
	}
}
