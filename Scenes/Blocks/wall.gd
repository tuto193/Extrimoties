extends GridPiece

class_name Wall

func _init() -> void:
	self.cell_type = GridTraits.CellType.WALL
