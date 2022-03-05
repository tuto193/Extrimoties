extends TileMap

class_name Grid

var player = Player

func _ready():
	for child in get_children():
		set_cellv(world_to_map(child.position),child.type)

func get_cell_content(coordinates):
	for node in get_children():
		if world_to_map(node.position) == coordinates:
			return(node)

func try_move_towards(piece, direction):
	var cell_start = world_to_map(piece.position)
	var cell_target = cell_start + direction

	var cell_target_type = get_cellv(cell_target)
	match cell_target_type:
		EMPTY:
			return update_piece_position(piece, cell_start, cell_target)
		OBJECT:
			var object_piece = get_cell_dude(cell_target)
			object_piece.queue_free()
			return update_piece_position(piece, cell_start, cell_target)
		ACTOR:
			var piece_name = get_cell_piece(cell_target).name
			print("Cell %s contains %s" % [cell_target, piece_name])

func update_piece_position(piece, cell_start, cell_target):
	set_cellv(cell_target, piece.type)
	set_cellv(cell_start, EMPTY)
	return map_to_world(cell_target) + cell_size / 2
