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


	private Grid _grid;

	private bool _can_move = true;

	public void _on_GridPiece_StartedMoving() {
		this._can_move = false;
	}

	public void _on_GridPiece_FinishedMoving() {
		this._can_move = true;
	}

	public Eye() {
		this._dimensions = new Vector2(64, 64) * this.Scale;
	}

	public override void _Ready() {
		this._grid = GetParent<Grid>();
		this._current_state = FaceDirFromString(this.Animation.ToUpper());
		Error _err = Connect(nameof(FinishedMoving), this, nameof(_on_GridPiece_FinishedMoving));
		_err = Connect(nameof(StartedMoving), this, nameof(_on_GridPiece_StartedMoving));
	}

	private FaceDir FaceDirFromString(String state_string) {
		switch (state_string) {
		case "UP":
			return FaceDir.Up;
		case "DOWN":
			return FaceDir.Down;
		case "LEFT":
			return FaceDir.Left;
		case "RIGHT":
			return FaceDir.Right;
		case "TOP":
			return FaceDir.Top;
		case "BOTTOM":
			return FaceDir.Bottom;
		default:
			throw new ArgumentOutOfRangeException(state_string, $"Not expected value: {state_string}");
		}
	}

	private String StringFromFaceDir(FaceDir fd) {
		switch (fd) {
			case FaceDir.Up:
				return "Up";
			case FaceDir.Down:
				return "Down";
			case FaceDir.Left:
				return "Left";
			case FaceDir.Right:
				return "Right";
			case FaceDir.Top:
				return "Top";
			case FaceDir.Bottom:
				return "Bottom";
			default:
				return "Up";
		}
	}

	private Vector2 GetInputDirection() {
		int left = Input.IsActionJustPressed("ui_left")? 1: 0;
		int right = Input.IsActionJustPressed("ui_right")? 1: 0;
		int x = right - left;
		if (x != 0) {
			return new Vector2(x, 0);
		}
		int up = Input.IsActionJustPressed("ui_up")? 1: 0;
		int down = Input.IsActionJustPressed("ui_down")? 1: 0;
		int y = down - up;
		return new Vector2(0, y);
	}

	private void UpdateState(Vector2 input_dir) {
		FaceDir new_direction = FaceDir.Top;
		if (input_dir.x != 0) {
			FaceDir[] new_directions = {FaceDir.Left, FaceDir.Right};
			new_direction = new_directions[(int) ((input_dir.x + 1) / 2)];
		}
		else {
			FaceDir[] new_directions = {FaceDir.Up, FaceDir.Down};
			new_direction = new_directions[(int) ((input_dir.y + 1) / 2)];
		}
		FaceDir old_state = _current_state;
		switch (_current_state) {
			case FaceDir.Top:
				_current_state = FaceDir.Bottom;
				break;
			// Parallel movements
			case FaceDir.Up:
				switch (new_direction) {
					case FaceDir.Up:
						_current_state = FaceDir.Bottom;
						break;
					case FaceDir.Down:
						_current_state = FaceDir.Top;
						break;
				}
				break;
			case FaceDir.Down:
				switch (new_direction) {
					case FaceDir.Up:
						_current_state = FaceDir.Top;
						break;
					case FaceDir.Down:
						_current_state = FaceDir.Bottom;
						break;
				}
				break;
			case FaceDir.Right:
				switch (new_direction) {
					case FaceDir.Right:
						_current_state = FaceDir.Bottom;
						break;
					case FaceDir.Left:
						_current_state = FaceDir.Top;
						break;
				}
				break;
			case FaceDir.Left:
				switch (new_direction) {
					case FaceDir.Right:
						_current_state = FaceDir.Top;
						break;
					case FaceDir.Left:
						_current_state = FaceDir.Bottom;
						break;
				}
				break;
			//////////////////////////////////////////////////
			case FaceDir.Bottom:
				switch (new_direction) {
					case FaceDir.Up:
						_current_state = FaceDir.Down;
						break;
					case FaceDir.Down:
						_current_state = FaceDir.Up;
						break;
					case FaceDir.Right:
						_current_state = FaceDir.Left;
						break;
					case FaceDir.Left:
						_current_state = FaceDir.Right;
						break;
				}
				break;
		}
		UpdateAnimation(old_state);
	}

	private void UpdateAnimation(FaceDir _old_state) {

		Animation = StringFromFaceDir(_current_state).ToLower();
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 direction_vector = GetInputDirection();
		if (direction_vector.Length() > 0 && _can_move) {
			Vector2 old_pos = this.Position;
			Vector2 new_pos = _grid.MovePieceTowards(this, direction_vector);
			if (new_pos != old_pos) {
				UpdateState(direction_vector);
			}
		}
	}
}
