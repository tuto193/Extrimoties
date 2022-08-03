using Godot;
using System;
using static GridTraits;

public class Goal : Hole {
    public override void StepIntoCheck(GridPiece gp)
    {
        if (gp.Cell_Type == CellType.Life) {
            // if (gp){ EmitSignal(nameof(ObjectEntered), gp)}
        }
    }
}
