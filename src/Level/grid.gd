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

func can_move_towards(piece, direction) -> bool:
	var cell_start: Vector2 = world_to_map(piece.position)
	var cell_target: Vector2 = cell_start + direction

	var cell_target_type = get_cellv(cell_target)
	match cell_target_type:
		GridTraits.CELL_TYPE.BOX:
			var object_piece: Node = get_cell_content(cell_target)
			# Commented out, since we want to move objects, not destroy them
			if piece.type == GridTraits.CELL_TYPE.BOX:
				print("Cell %s already contains a box. Cannot push")
				return false
			if not can_move_towards(
				object_piece,
				cell_target + direction
			):
				return false
			return true
		GridTraits.CELL_TYPE.WALL:
			print("Cell %s contains a wall")
			return false
		_: # Can move to new position
			return true


func try_move_towards(piece: Node, direction: Vector2):
	var cell_start = world_to_map(piece.position)
	var cell_target = cell_start + direction

	var cell_target_type = get_cellv(cell_target)
	match cell_target_type:
		GridTraits.CELL_TYPE.BOX:
			var object_piece: Node = get_cell_content(cell_target)
			# Commented out, since we want to move objects, not destroy them
			if piece.type == GridTraits.CELL_TYPE.BOX:
				print("Cell %s already contains a box. Cannot push")
				return null
			# object_piece.queue_free()
			var new_object_pos: Vector2 = try_move_towards(
				object_piece,
				cell_target + direction
			)
			if not new_object_pos:
				return null
			# TODO: maybe make a function for the object, where it can be
			# moved smoothly
			object_piece.position = new_object_pos
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
