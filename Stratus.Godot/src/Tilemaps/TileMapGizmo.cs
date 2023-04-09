using Godot;

using Stratus.Godot.Extensions;
using Stratus.Godot.Inputs;
using Stratus.Godot.UI;

namespace Stratus.Godot.TileMaps
{
	public abstract partial class MapNode : Node2D
	{
		public MapManager map { get; private set; }

		public void Initialize(MapManager manager)
		{
			this.Log($"Initialized by {manager}");
			this.map = manager;
		}
	}

	public abstract partial class TileMapGizmo : MapNode
	{
		private MenuInputLayer input;

		public bool active { get; private set; }

		protected abstract void Move(Vector2I input);
		protected abstract void OnConfirm();
		protected abstract void OnCancel();

		public override void _Ready()
		{
			input = new MenuInputLayer(Name);
			input.move += Move;
			input.select += Confirm;
			input.cancel += Cancel;
		}

		protected void ShowGizmo()
		{
			this.Log("Showing");
			input.Push();
			Visible = true;
			active = true;
		}

		protected void HideGizmo()
		{
			this.Log("Hiding");
			input.Pop();
			Visible = false;
			active = false;
		}

		public void Confirm()
		{
			this.Log("Confirm");
			OnConfirm();
			HideGizmo();
		}

		public void Cancel()
		{
			this.Log("Cancel");
			OnCancel();
			HideGizmo();
		}
	}
}
