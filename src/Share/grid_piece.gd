extends StaticBody2D

class_name GridPiece

onready var grid: Grid = get_parent()


# Basic move function:
# TODO: expand once animations are implemented
func move_to(new_pos: Vector2) -> void:
	self.position = new_pos
