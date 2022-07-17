use gdnative::api::*;
// use gdnative::object::ownership;
use gdnative::prelude::*;

use crate::grid_piece::GridPiece;

/// The Game "class"
#[derive(NativeClass)]
#[inherit(TileMap)]
#[register_with(Self::register_builder)]
pub struct Grid;


// __One__ `impl` block can have the `#[methods]` attribute, which will generate
// code to automatically bind any exported methods to Godot.
#[methods]
impl Grid {
    // Register the builder for methods, properties and/or signals.
    fn register_builder(_builder: &ClassBuilder<Self>) {
        godot_print!("Grid builder is registered!");
    }

    /// The "constructor" of the class.
    fn new(_owner: &TileMap) -> Self {
        godot_print!("Grid is being created!");
        Grid {}
    }

    // In order to make a method known to Godot, the #[export] attribute has to be used.
    // In Godot script-classes do not actually inherit the parent class.
    // Instead they are "attached" to the parent object, called the "owner".
    // The owner is passed to every single exposed method.
    #[export]
    fn _ready(&mut self, owner: &TileMap) {
        // godot_print!("{} is ready!", self.name);
        for child in &owner.get_children() {
            &owner.set_cellv(owner.world_to_map(child.to_object::<Node2D>().unwrap().position()), child.to_object::<GridPiece>().unwrap().cell_type, false, false, false);
        }
    }
}
