using Godot;
using System;
using static GridTraits;

public class GridPiece : AnimatedSprite {
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


	public virtual CellType Cell_Type {
		get;
		// private set { _cell_type = value;}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
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

	public bool EnterHole(Hole h) {
		this.IsStandingOn = h.Cell_Type;
		if (IsStandingOn == CellType.Hole) {
			EmitSignal(nameof(FallInHole));
			return true;
		}
		return false;
	}

	public bool ExitHole(Hole h) {
		// this.IsStandingOn =
		return false;
	}

	public virtual async void PlayFallInHole() {
		SceneTreeTween tween = GetTree()
			.CreateTween()
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);
	}
}
