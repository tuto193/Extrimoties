using Godot;
using System;
using static GridTraits;

public class Hole : GridPiece {
    [Signal]
    delegate void ObjectEntered(GridPiece gp);

    // public override void _Init()
    // {

    // }

    public virtual void StepIntoCheck(GridPiece gp)
    {
        CellType[] checks = {CellType.Box, CellType.Life};
        if (Array.IndexOf(checks, gp.Cell_Type) > -1)
        {
            EmitSignal(nameof(ObjectEntered), gp);
        }
    }
}
