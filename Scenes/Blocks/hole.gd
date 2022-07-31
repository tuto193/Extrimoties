class_name Hole
extends GridPiece

signal object_entered(object)

func _init(c_t = GridTraits.CellType.HOLE).(c_t) -> void:
	pass



# @virtual
func step_into_check(object) -> void:
	if object.cell_type in [GridTraits.CellType.BOX, GridTraits.CellType.LIFE]:
		.emit_signal("object_entered", object)
