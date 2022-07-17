class_name Goal
extends GridPiece

var type = GridTrais.CellType.GOAL

func step_into_check(object: GridPiece) -> void:
    if object is Eye:
        emit_signal("object_entered", object)
