using Godot;
using System;
using static GridTraits;

public class GridPiece : AnimatedSprite {
	private AnimationPlayer _animation_player;

	protected CellType _is_standing_on = CellType.Empty;

	public CellType IsStandingOn {
		get {return _is_standing_on;}
		set { _is_standing_on = value;}
	}

	private Vector2 _initial_position;

	public Vector2 InitialPosition {
		get {return _initial_position;}
		set {_initial_position = value;}
	}

	[Export]
	private float _time_animation = 0.25f;

	// Signals
	[Signal]
	protected delegate void StartedMoving();

	[Signal]
	protected delegate void FinishedMoving();

	[Signal]
	protected delegate void FallInHole();

	private CellType _cell_type = CellType.Empty;

	public virtual CellType Cell_Type {
		get {return _cell_type;}
		set {_cell_type = value;}
	}



	// public GridPiece(CellType ct = CellType.Empty) {
	// 	this._cell_type = ct;
	// }

	public void ReInitialize(CellType ct) {
		// GD.Print("In ReInitialize()");
		Cell_Type = ct;
		// Animation = $"{StringFromCellType(_cell_type).ToLower()}_idle";
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		this._animation_player = GetNode<AnimationPlayer>("AnimationPlayer");
		Animation = $"{StringFromCellType(Cell_Type).ToLower()}_idle";
	}

	public static CellType CellTypeFromString(String ct) {
		switch (ct.ToUpper()) {
			case "EMPTY":
				return CellType.Empty;
			case "BOX":
				return CellType.Box;
			case "WALL":
				return CellType.Wall;
			case "LIFE":
				return CellType.Life;
			case "GOAL":
				return CellType.Goal;
			case "HOLE":
				return CellType.Hole;
			default:
				throw new ArgumentOutOfRangeException(ct, $"Not expected value: {ct}\n");
		}
	}

	public static String StringFromCellType(CellType ct) {
		switch (ct) {
			case CellType.Empty:
				return "Empty";
			case CellType.Box:
				return "Box";
			case CellType.Wall:
				return "Wall";
			case CellType.Life:
				return "Life";
			case CellType.Goal:
				return "Goal";
			case CellType.Hole:
				return "Hole";
			default:
				throw new ArgumentOutOfRangeException(ct.ToString(), $"Unexpected value: {ct}");
		}
	}

	public virtual async void MoveTo(Vector2 new_pos, CellType np_type = CellType.Empty) {
		EmitSignal(nameof(StartedMoving));
		SetProcess(false);
		SceneTreeTween _tween = GetTree()
			.CreateTween()
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);
		_tween.TweenProperty(this, "position", new_pos, _time_animation);
		// bool _unused = _tween.InterpolateProperty(
		// 	this,
		// 	"Position",
		// 	Position,
		// 	new_pos,
		// 	_time_animation,
		// 	Tween.TransitionType.Sine,
		// 	Tween.EaseType.Out
		// );
		// _unused = _tween.Start();
		await ToSignal(_tween, "finished");
		this.IsStandingOn = np_type;
		SetProcess(true);
		EmitSignal(nameof(FinishedMoving));
	}

	public bool EnterHole(GridPiece h) {
		this.IsStandingOn = h.Cell_Type;
		if (IsStandingOn == CellType.Hole) {
			EmitSignal(nameof(FallInHole));
			return true;
		}
		return false;
	}

	public bool ExitHole(GridPiece h) {
		// this.IsStandingOn =
		return false;
	}

	public virtual async void PlayFallInHole() {

	}
}
