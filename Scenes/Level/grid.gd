class_name Grid
extends TileMap


func _ready():
	for child in get_children():
		set_cellv(world_to_map(child.position),child.type)

func get_cell_content(coordinates: Vector2) -> StaticBody2D:
	for node in get_children():
		if world_to_map(node.position) == coordinates:
			return(node)
	return null


# Attempts to move `piece` towards `direction`.
# Returns the `new_pos` where the piece should now be.
func move_piece_towards(piece: StaticBody2D, direction: Vector2) -> Vector2:
	var cell_map_start: Vector2 = world_to_map(piece.position)
	var cell_map_target: Vector2 = cell_map_start + direction

	var cell_map_target_type = get_cellv(cell_map_target)
	match cell_map_target_type:
		GridTraits.CELL_TYPE.BOX:
			var object_piece: StaticBody2D= get_cell_content(cell_map_target)
			# Commented out, since we want to move objects, not destroy them
			if piece.type == GridTraits.CELL_TYPE.BOX:
				print("Cell %s already contains a box. Cannot push" %(cell_map_target))
				return piece.position
			var object_world_start: Vector2 = object_piece.position
			var object_new_pos: Vector2 = move_piece_towards(
				object_piece,
				direction
			)
			if object_world_start == object_new_pos:
				return piece.position # didn't move
			# Object was already successfully moved, so we don't need to
			# update any more stuff from it
			var new_pos: Vector2 = update_grid_positions(
				piece,
				cell_map_start,
				cell_map_target
			)
			piece.move_to(new_pos)
			return new_pos
		GridTraits.CELL_TYPE.WALL:
			print("Cell %s contains a wall" %(cell_map_target))
			return piece.position # didn't move the piece. Return origin
		_: # Can move to new position
			var new_pos: Vector2 = update_grid_positions(
				piece,
				cell_map_start,
				cell_map_target
			)
			piece.move_to(new_pos)
			return new_pos

func update_grid_positions(piece, cell_map_start, cell_map_target) -> Vector2:
	set_cellv(cell_map_target, piece.type)
	set_cellv(cell_map_start, GridTraits.CELL_TYPE.EMPTY)
	return map_to_world(cell_map_target) + cell_size / 2
