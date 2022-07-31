class_name Goal
extends GridPiece

func _init() -> void:
	self.cell_type = GridTraits.CellType.GOAL

func step_into_check(object: GridPiece) -> void:
	if object is Eye:
		emit_signal("object_entered", object)
