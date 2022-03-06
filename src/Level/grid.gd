extends TileMap

class_name Grid


func _ready():
	for child in get_children():
		set_cellv(world_to_map(child.position),child.type)

func get_cell_content(coordinates) -> Node:
	for node in get_children():
		if world_to_map(node.position) == coordinates:
			return(node)
	return null

func try_move_towards(piece, direction):
	var cell_start = world_to_map(piece.position)
	var cell_target = cell_start + direction

	var cell_target_type = get_cellv(cell_target)
	match cell_target_type:
		GridTraits.CELL_TYPE.BOX:
			var object_piece = get_cell_content(cell_target)
			object_piece.queue_free()
			return update_piece_position(piece, cell_start, cell_target)
		GridTraits.CELL_TYPE.WALL:
			print("Cell %s contains a wall")
			return null
		_: # Can move to new position
			return update_piece_position(piece, cell_start, cell_target)

func update_piece_position(piece, cell_start, cell_target) -> Vector2:
	set_cellv(cell_target, piece.type)
	set_cellv(cell_start, GridTraits.CELL_TYPE.EMPTY)
	return map_to_world(cell_target) + cell_size / 2
