using Godot;
using System;
using static GridTraits;

public class GridPiece : StaticBody2D
{
	private Tween _tween;

	protected CellType _is_standing_on;

	public CellType IsStandingOn {
		get {return _is_standing_on;}
		set { _is_standing_on = value;}
	}

	[Export]
	private float _time_animation;

	// Signals
	[Signal]
	delegate void StartedMoving();

	[Signal]
	delegate void FinishedMoving();

	[Signal]
	delegate void FallInHole();


	public virtual CellType Cell_Type {
		get;
		// private set { _cell_type = value;}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._tween = GetNode<Tween>("Tween");
	}

	public virtual async void MoveTo(Vector2 new_pos)
	{
		EmitSignal(nameof(StartedMoving));
		SetProcess(false);
		bool _unused = _tween.InterpolateProperty(
			this,
			"Position",
			Position,
			new_pos,
			_time_animation,
			Tween.TransitionType.Sine,
			Tween.EaseType.Out
		);
		_unused = _tween.Start();
		await ToSignal(_tween, "tween_completed");
		SetProcess(true);
		EmitSignal(nameof(FinishedMoving));
	}

	public virtual bool HandleHole(GridPiece _hole)
	{
		EmitSignal(nameof(FallInHole));
		return true;
	}
}
