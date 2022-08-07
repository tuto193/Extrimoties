using Godot;
using System;
using static GridTraits;

public class Goal : Hole {
    public override CellType Cell_Type { get { return _cell_type; } }

    public Goal() { this._cell_type = CellType.Goal; }

    public override void StepIntoCheck(GridPiece gp) {
        if (gp.Cell_Type == CellType.Life) {
            if (gp is Eye) {
                EmitSignal(nameof(ObjectEntered), gp);
            }
        }
    }
}
