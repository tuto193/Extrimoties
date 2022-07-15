class_name Hole
extends GridPiece

var type = GridTraits.CellType.EMPTY

signal object_entered(object)

# @virtual
func step_into_check(object: GridPiece) -> void:
	if object.type in [GridTraits.CellType.BOX, GridTraits.CellType.LIFE]:
		emit_signal("object_entered", object)
