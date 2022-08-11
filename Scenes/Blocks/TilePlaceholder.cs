using Godot;
using System;

public class TilePlaceholder : Sprite
{

	public override void _Draw()
	{
		if (Engine.EditorHint) {
			this.Visible = false;
			return;
		}
		this.Visible = true;
	}
}
