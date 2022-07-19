use gdnative::api::*;
// use gdnative::object::ownership;
use gdnative::prelude::*;

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
    fn _ready(&mut self, owner: &TileMap) {
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
                .map(|grid_piece, _owner| {
                    // TODO: fix this mapping... somehow
                    grid_piece.cell_type
                })
                .expect("Failed to map over grid_piece instance");
            // There is some output that we can simply ignore
            let _ = &owner.set_cellv(owner.world_to_map(position), cell_type as i64, false, false, false);
            // let cell_type = grid_piece.cell_type;
        }
    }
}
