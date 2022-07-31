class_name Hole
extends GridPiece

signal object_entered(object)

func _init() -> void:
	self.cell_type = GridTraits.CellType.EMPTY


# @virtual
func step_into_check(object: GridPiece) -> void:
	if object.cell_type in [GridTraits.CellType.BOX, GridTraits.CellType.LIFE]:
		emit_signal("object_entered", object)
