class_name GridPiece
extends StaticBody2D

onready var grid: Grid = get_parent()
onready var tween: Tween = $Tween
export var time_animation: float = 0.25

signal started_moving
signal finished_moving

# Basic move function:
# TODO: expand once animations are implemented
func move_to(new_pos: Vector2) -> void:
	emit_signal("started_moving")
	set_process(false)
	var _unused = tween.interpolate_property(
		self,
		"position",
		self.position,
		new_pos,
		time_animation,
		Tween.TRANS_SINE,
		Tween.EASE_OUT
	)
	_unused = tween.start()
	yield(tween, "tween_completed")
	set_process(true)
	emit_signal("finished_moving")
	# emit_signal("finished_moving")
	# self.position = new_pos
