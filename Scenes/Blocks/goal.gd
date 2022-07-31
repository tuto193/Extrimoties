class_name Goal
extends GridPiece

func _init(c_t = GridTraits.CellType.GOAL).(c_t) -> void:
	pass


func step_into_check(object: GridPiece) -> void:
	if object is Eye:
		emit_signal("object_entered", object)
