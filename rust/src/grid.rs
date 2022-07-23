use gdnative::api::*;
// use gdnative::object::ownership;
use gdnative::prelude::*;

use crate::grid_piece::CellType;
use crate::grid_piece::GridPiece;

/// The Game "class"
#[derive(NativeClass)]
#[inherit(TileMap)]
#[register_with(Self::register_builder)]
pub struct Grid;

#[methods]
impl Grid {
    // Register the builder for methods, properties and/or signals.
    fn register_builder(_builder: &ClassBuilder<Self>) {
        godot_print!("Grid builder is registered!");
    }

    /// The "constructor" of the class.
    fn new(_owner: &TileMap) -> Self {
        godot_print!("Grid is being created!");
        Self {}
    }

    // In order to make a method known to Godot, the #[export] attribute has to be used.
    // In Godot script-classes do not actually inherit the parent class.
    // Instead they are "attached" to the parent object, called the "owner".
    // The owner is passed to every single exposed method.
    #[export]
    fn _ready(&mut self, owner: TRef<TileMap>) {
        // godot_print!("{} is ready!", self.name);
        for child in &owner.get_children() {
            let node2d = unsafe {
                child
                    .to_object::<StaticBody2D>()
                    .expect("All children of Grid must be of type 'StaticBody' at least")
                    .assume_safe()
            };
            let position = node2d.position();
            let grid_piece = node2d
                .cast_instance::<GridPiece>()
                .expect("Child node has to be of type 'GridPiece'");
            let cell_type = grid_piece
                .map(|grid_piece, _owner| grid_piece.cell_type)
                .expect("Failed to map over grid_piece instance");
            // There is some output that we can simply ignore
            let _ = &owner.set_cellv(
                owner.world_to_map(position),
                cell_type as i64,
                false,
                false,
                false,
            );
        }
    }

    fn get_cell_content(
        &self,
        owner: TRef<TileMap>,
        coords: Vector2,
    ) -> Option<Instance<GridPiece>> {
        for node in &owner.get_children() {
            let grid_piece = unsafe {
                node.to_object::<StaticBody2D>()
                    .expect("All children of Grid should at least inherit `StaticBody2D")
                    .assume_safe()
            };
            if owner.world_to_map(grid_piece.position()) == coords {
                let grid_piece = grid_piece
                    .cast_instance::<GridPiece>()
                    .expect("All children of Grid are `GridPiece`s");
                return Some(grid_piece.claim());
            }
        }
        None
    }

    fn upgrade_grid_positions(
        &self,
        owner: TRef<TileMap>,
        piece: TInstance<GridPiece>,
        start: Vector2,
        target: Vector2,
    ) -> Vector2 {
        owner.set_cellv(
            target,
            piece.map(|p, _o| p.cell_type).unwrap() as i64,
            false,
            false,
            false,
        );
        owner.set_cellv(start, CellType::Empty as i64, false, false, false);
        owner.map_to_world(target, false) + (owner.cell_size() / 2.0)
    }

    pub fn move_piece_towards(
        &self,
        owner: TRef<TileMap>,
        piece: TInstance<GridPiece>,
        direction: Vector2,
    ) -> Vector2 {
        let piece_position = piece.base().position();
        let cell_map_start = owner.world_to_map(piece_position);
        let cell_map_target = cell_map_start + direction;
        let target_cell_type = CellType::from_i64(owner.get_cellv(cell_map_target)).unwrap();
        // let target_cell_type = match target_cell_type {
        //     Ok(t) => t,
        //     Err(e) => godot_error!("Error when getting enum: {:?}", e),
        // };
        match target_cell_type {
            CellType::Box => {
                let target_piece = &self.get_cell_content(owner, cell_map_target).unwrap();
                let target_piece = unsafe { target_piece.assume_safe() };
                let moving_piece_type = piece.map(|p, o| p.cell_type).unwrap();
                // Boxes cannot push each other (for now?)
                if moving_piece_type == CellType::Box {
                    godot_print!(
                        "Cell {:?} already contains a box. Cannot push",
                        cell_map_target
                    );
                    return piece_position;
                }
                let box_start_position = target_piece.base().position();
                let box_end_pos = self.move_piece_towards(owner, target_piece, direction);
                if box_start_position == box_end_pos {
                    return piece_position;
                }
                let new_position =
                    self.upgrade_grid_positions(owner, piece, cell_map_start, cell_map_target);
                piece.map(|p, _o| p.move_to(_o));

                return Vector2::new(0.0, 0.0);
            }
            CellType::Empty => todo!(),
            CellType::Wall => todo!(),
            CellType::Life => todo!(),
            CellType::Goal => todo!(),
        }
    }
}
