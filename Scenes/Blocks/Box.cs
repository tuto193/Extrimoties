using Godot;
using System;
using static GridTraits;

public class Box : GridPiece {
	private CellType _cell_type = CellType.Box;

	public override CellType Cell_Type { get { return _cell_type; } }
}
