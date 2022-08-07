using Godot;
using System;
using static GridTraits;

public class Wall : GridPiece {
	private CellType _cell_type = CellType.Wall;

	public override CellType Cell_Type { get { return _cell_type; } }
}
