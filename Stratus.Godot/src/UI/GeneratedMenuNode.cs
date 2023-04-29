using Godot;

using Stratus.Models.UI;

namespace Stratus.Godot.UI
{
	public abstract partial class GeneratedMenuNode : Control
	{
		protected abstract Container container { get; }
		private InputLayeredMenuGenerator generator;

		public override void _Ready()
		{
			generator = new InputLayeredMenuGenerator(this, container);
			generator.Close();
		}

		protected void Open(Menu menu)
		{
			generator.Open(menu);
		}
	}
}
