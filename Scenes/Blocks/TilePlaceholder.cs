using Godot;
using System;

public class TilePlaceholder : Sprite
{

	public override void _Draw()
	{
		if (Engine.EditorHint) {
			this.Visible = true;
			return;
		}
		this.Visible = false;
	}
}
