extends GridPiece

class_name Eye


# To differentiate from others in grid
var type = GridTraits.CELL_TYPE.LIFE

# speed of movement in grid
export var move_delay: float = 1

# sprite is 32x32, scaled to 0.5 on both axes
var dimensions: Vector2 = Vector2(64, 64) * self.scale
# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
# func _ready():
# 	pass # Replace with function body.


func get_input_direction() -> Vector2:
	return Vector2(
		int(Input.is_action_just_pressed("ui_right"))
		- int(Input.is_action_just_pressed("ui_left")),
		int(Input.is_action_just_pressed("ui_down"))
		- int(Input.is_action_just_pressed("ui_up"))
	)



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(_delta) -> void:
	var direction_vector: Vector2 = get_input_direction()
	# No type hinting, since it throws errors in case of null
	if direction_vector.length() > 0:
		# Not neccessary right now
		var _new_pos: Vector2 = grid.move_piece_towards(self, direction_vector)
