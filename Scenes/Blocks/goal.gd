class_name Goal
extends GridPiece

func _init(c_t = GridTraits.CellType.GOAL).(c_t) -> void:
	pass


func step_into_check(object) -> void:
	if object.cell_type == GridTraits.CellType.LIFE:
		.emit_signal("object_entered", object)
