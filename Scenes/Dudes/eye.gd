class_name Eye
extends GridPiece



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

onready var animated_sprite: AnimatedSprite

var _can_move: bool = true

func _init(c_t = GridTraits.CellType.LIFE).(c_t) -> void:
	pass


func _on_GridPiece_started_moving() ->void:
	self._can_move = false

func _on_GridPiece_finished_moving() ->void:
	self._can_move = true

func _ready():
	# Spicy initialization, but works
	self.animated_sprite = .get_node("AnimatedSprite")
	self.current_state = FaceDir[animated_sprite.animation.to_upper()]
	var _err = .connect("started_moving", self, "_on_GridPiece_started_moving")
	_err = .connect("finished_moving", self, "_on_GridPiece_finished_moving")

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

# func _on_GridPiece_finished_moving():


# Updates the facing direction of 'current_state' based on the 'input_dir'
# so it rolls like a die.
func _update_state(input_dir: Vector2) -> void:
	var new_direction = FaceDir.TOP  # unvalid direction
	if input_dir.x != 0:
		new_direction = [FaceDir.LEFT, FaceDir.RIGHT][
			int((input_dir.x + 1) / 2)
		]
	else:  # moving vertically
		new_direction = [FaceDir.UP, FaceDir.DOWN][
			int((input_dir.y + 1) / 2)
		]
	var old_state = current_state
	match current_state:
		FaceDir.TOP:
			self.current_state = new_direction
		##################################
		# Parallel movements
		FaceDir.UP:
			match new_direction:
				FaceDir.UP:
					self.current_state = FaceDir.BOTTOM
				FaceDir.DOWN:
					self.current_state = FaceDir.TOP
		FaceDir.DOWN:
			match new_direction:
				FaceDir.UP:
					self.current_state = FaceDir.TOP
				FaceDir.DOWN:
					self.current_state = FaceDir.BOTTOM
		FaceDir.RIGHT:
			match new_direction:
				FaceDir.RIGHT:
					self.current_state = FaceDir.BOTTOM
				FaceDir.LEFT:
					self.current_state = FaceDir.TOP
		FaceDir.LEFT:
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
	_update_animation(old_state)


func _update_animation(_old_state) -> void:
	animated_sprite.animation = FaceDir.keys()[current_state].to_lower()
	# animated_sprite.stop()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(_delta) -> void:
	var direction_vector: Vector2 = get_input_direction()
	# No type hinting, since it throws errors in case of null
	if direction_vector.length() > 0 and _can_move:
		# Not neccessary right now
		var old_pos = self.position
		var new_pos: Vector2 = self.grid.move_piece_towards(self, direction_vector)
		if old_pos != new_pos:
			_update_state(direction_vector)
