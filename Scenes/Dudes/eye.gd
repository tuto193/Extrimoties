class_name Eye
extends GridPiece


# To differentiate from others in grid
var type = GridTraits.CellType.LIFE

# speed of movement in grid
export var move_delay: float = 1

# sprite is 32x32, scaled to 0.5 on both axes
var dimensions: Vector2 = Vector2(64, 64) * self.scale

enum FaceDir {
	UP,
	DOWN,
	LEFT,
	RIGHT,
	TOP,
	BOTTOM,
}

# Current looking direction of the eye
onready var current_state

onready var animated_sprite: AnimatedSprite = $AnimatedSprite

func _ready():
	self.current_state = FaceDir.BOTTOM

# Returns the input direction. It can only be one of the four main ones
# and no diagonal vectores are allowed/returned.
func get_input_direction() -> Vector2:
	var x_direction: int = (
		int(Input.is_action_just_pressed("ui_right"))
		- int(Input.is_action_just_pressed("ui_left"))
	)
	if x_direction != 0:
		return Vector2(x_direction, 0)
	var y_direction: int = (
		int(Input.is_action_just_pressed("ui_down"))
		- int(Input.is_action_just_pressed("ui_up"))
	)
	return Vector2(0, y_direction)


# Updates the facing direction of 'current_state' based on the 'input_dir'
# so it rolls like a die.
func update_state(input_dir: Vector2) -> void:
	var new_direction = FaceDir.TOP  # unvalid direction
	if input_dir.x != 0:
		new_direction = [FaceDir.LEFT, FaceDir.RIGHT][
			int((input_dir.x + 1) / 2)
		]
	else:  # moving vertically
		new_direction = [FaceDir.UP, FaceDir.DOWN][
			int((input_dir.y + 1) / 2)
		]
	print("update_state(): current_state = %s" %FaceDir.keys()[current_state])
	var old_state = current_state
	match current_state:
		FaceDir.TOP:
			self.current_state = new_direction
		##################################
		# Parallel movements
		FaceDir.UP:
			print("update_state(): here...")
			match new_direction:
				FaceDir.UP:
					self.current_state = FaceDir.BOTTOM
				FaceDir.DOWN:
					self.current_state = FaceDir.TOP
		FaceDir.DOWN:
			print("update_state(): here...")
			match new_direction:
				FaceDir.UP:
					self.current_state = FaceDir.TOP
				FaceDir.DOWN:
					self.current_state = FaceDir.BOTTOM
		FaceDir.RIGHT:
			print("update_state(): here...")
			match new_direction:
				FaceDir.RIGHT:
					self.current_state = FaceDir.BOTTOM
				FaceDir.LEFT:
					self.current_state = FaceDir.TOP
		FaceDir.LEFT:
			print("update_state(): here...")
			match new_direction:
				FaceDir.RIGHT:
					self.current_state = FaceDir.TOP
				FaceDir.LEFT:
					self.current_state = FaceDir.BOTTOM
		####################################
		FaceDir.BOTTOM:
			match new_direction:
				FaceDir.UP:
					self.current_state = FaceDir.DOWN
				FaceDir.DOWN:
					self.current_state = FaceDir.UP
				FaceDir.RIGHT:
					self.current_state = FaceDir.LEFT
				FaceDir.LEFT:
					self.current_state = FaceDir.RIGHT
		# handle perpendicular movements
		FaceDir.UP, FaceDir.DOWN:
			match new_direction:
				FaceDir.LEFT, FaceDir.RIGHT:
					# do nothing
					print("update_state(): Perpendicular movement (horizontal)")
				_:  # not perpendicular
					print("update_state(): Parallel movement continue to..")
					continue
		FaceDir.LEFT, FaceDir.RIGHT:
			match new_direction:
				FaceDir.UP, FaceDir.DOWN:
					# do nothing
					print("update_state(): Perpendicular movement (vertical)")
				_:  # not perpendicular
					print("update_state(): Parallel movement continue to..")
					continue

	_update_animation(old_state)


func _update_animation(_old_state) -> void:
	print("_update_animation(): Updating animation from %s" %FaceDir.keys()[_old_state])
	print("_update_animation(): Current state is %s" %FaceDir.keys()[current_state])
	animated_sprite.animation = FaceDir.keys()[current_state].to_lower()
	# animated_sprite.stop()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(_delta) -> void:
	var direction_vector: Vector2 = get_input_direction()
	# No type hinting, since it throws errors in case of null
	if direction_vector.length() > 0:
		# Not neccessary right now
		var old_pos = position
		var new_pos: Vector2 = grid.move_piece_towards(self, direction_vector)
		if old_pos != new_pos:
			print("_physics_process(): Updating state to %s" %direction_vector)
			update_state(direction_vector)
