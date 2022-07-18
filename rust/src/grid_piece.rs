use gdnative::api::*;
// use gdnative::object::ownership;
use gdnative::prelude::*;

use crate::grid::Grid;

pub enum CellType {
    Empty,
    Wall,
    Box,
    Life,
    Goal,
}

#[derive(NativeClass)]
#[inherit(StaticBody2D)]
#[register_with(Self::register_signals)]
pub struct GridPiece {
    grid: Option<Instance<Grid>>,
    tween: Option<Ref<Tween>>,
    #[property(default = 0.25)]
    time_animation: f32,
    pub cell_type: CellType,
}

#[methods]
impl GridPiece {
    fn register_signals(builder: &ClassBuilder<Self>) {
        builder
            .signal("started_moving")
            .done();
        builder
            .signal("finished_moving")
            .done();
        builder
            .signal("fall_in_hole")
            .done();
        builder
            .signal("goal_reached")
            .done();
    }

    fn new(owner: &StaticBody2D) -> Self {
        Self {
            grid: None,
            tween: None,
            time_animation: 0.25,
            cell_type: CellType::Empty,
        }
    }

    #[export]
    fn _ready(&self, owner: &StaticBody2D) {
        // Initialize the onready vars
        let grid = unsafe {
            owner
                .get_parent()
                .expect("This node can only be instantiated within a Grid")
                .assume_safe()
        };
        let grid = grid
            .cast::<TileMap>()
            .expect("Parent node must be of type `TileMap` as base.")
            .cast_instance::<Grid>()
            .expect("Parent node must be of type `Grid` at heart.");
        self.grid = Some(grid.claim());
        let tween = owner
            .get_node("Tween")
            .expect("This node must have a child with the path 'Tween'");
        let tween = unsafe { tween.assume_safe() };
        let tween = tween
            .cast::<Tween>()
            .expect("Child must be of type 'Tween'");
        self.tween = Some(tween.claim());
    }

    pub fn move_to(&mut self, owner: &StaticBody2D) {
        owner.emit_signal("started_moving", &[]);
    }
}
