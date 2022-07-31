class_name GridPiece
extends StaticBody2D

var cell_type

onready var grid: Grid = get_parent()
onready var tween: Tween = $Tween
export var time_animation: float = 0.25
var is_standing_on = GridTraits.CellType.EMPTY

# Basic movement
signal started_moving
signal finished_moving

# Notify about falling to deal with freeing and animations
signal fall_in_hole

func _init(c_t = GridTraits.CellType.EMPTY) -> void:
	self.cell_type = c_t

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


# Deal with collisions when there's a hole. Returns 'true' if falls in hole
# Landing on a hole, will make the
# piece disappear
func enter_hole(hole: GridPiece) -> bool:
	self.is_standing_on = hole
	emit_signal("fall_in_hole")
	return true

func exit_hole(_hole: GridPiece) -> void:
	self.is_on_hole = false
