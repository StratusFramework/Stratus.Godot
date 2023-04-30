﻿using Godot;

using Stratus.Models.UI;

namespace Stratus.Godot.UI
{
	public abstract partial class GeneratedMenuNode : Control
	{
		protected abstract Container container { get; }
		protected InputLayeredMenuGenerator menu { get; private set; }

		public override void _Ready()
		{
			menu = new InputLayeredMenuGenerator(this, container);
			menu.Close();
		}

		protected void Open(Menu _menu)
		{
			menu.Open(_menu);
		}
	}
}
