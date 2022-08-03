using Godot;
using System;
using static GridTraits;

public class Eye : GridPiece {
    [Export]
    private float _move_delay = 1.0f;

    private Vector2 _dimensions;
    private CellType _cell_type = CellType.Life;
    public override CellType Cell_Type {
        get {return _cell_type;}
    }

    private enum FaceDir {
        Up,
        Down,
        Left,
        Right,
        Top,
        Bottom,
    }

    private FaceDir _current_state;

    private AnimatedSprite _animated_sprite;

    private bool _can_move = true;

    public Eye() {
        this._dimensions = new Vector2(64, 64) * this.Scale;
    }
}
