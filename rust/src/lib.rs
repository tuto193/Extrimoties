mod grid;
mod grid_piece;

use gdnative::prelude::{godot_init, InitHandle};

// Function that registers all exposed classes to Godot
fn init(handle: InitHandle) {
    handle.add_class::<grid::Grid>();
    handle.add_class::<grid_piece::GridPiece>();
}

// macros that create the entry-points of the dynamic library.
godot_init!(init);
