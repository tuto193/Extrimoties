class_name Goal
extends Hole

func step_into_check(object: GridPiece) -> void:
    if object is Eye:
        emit_signal("object_entered", object)
