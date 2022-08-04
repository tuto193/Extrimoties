using Godot;
using System;
using static GridTraits;

public class Hole : GridPiece {
    [Signal]
    protected delegate void ObjectEntered(GridPiece gp);

    protected CellType _cell_type = CellType.Hole;

    public override CellType Cell_Type { get{return _cell_type;} }

    public virtual void StepIntoCheck(GridPiece gp)
    {
        CellType[] checks = {CellType.Box, CellType.Life};
        if (Array.IndexOf(checks, gp.Cell_Type) > -1)
        {
            EmitSignal(nameof(ObjectEntered), gp);
        }
    }
}
