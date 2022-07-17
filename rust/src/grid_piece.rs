use gdnative::api::*;
use gdnative::object::ownership;
use gdnative::prelude::*;

use crate::grid::Grid;

pub enum CellType {
    EMPTY,
    WALL,
    BOX,
    LIFE,
    GOAL,
}

#[derive(NativeClass)]
#[inherit(StaticBody2D)]
#[register_with(Self::register_signals)]
pub struct GridPiece {
    grid: Option<Ref<Grid>>,
    tween: Option<Ref<Tween>>,
    #[property(default = 0.25)]
    time_animation: f32,
    cell_type: CellType,
}


#[methods]
impl GridPiece {
    fn register_signals(builder: &ClassBuilder<Self>) {
        builder.add_signal(Signal { name: "started_moving", args: &[] });
        builder.add_signal(Signal { name: "finished_moving", args: &[] });
        builder.add_signal(Signal { name: "fall_in_hole", args: &[] });
        builder.add_signal(Signal { name: "goal_reached", args: &[] });
    }

    #[export]
    fn _ready(&self, owner: &StaticBody2D) {
        // Initialize the onready vars
        let grid = owner
            .get_parent()
            .expect("This node can only be instantiated within a Grid");
        let grid = unsafe { grid.assume_safe() };
        let grid = grid.cast::<Grid>();
        self.grid = Some(grid.claim());
        let tween = owner
            .get_node("Tween")
            .expect("This node must have a child with the path 'Tween'");
        let tween = unsafe { tween.assume_safe() };
        let tween = tween.cast::<Tween>();
        self.tween = Some(tween.claim());
    }

    pub fn move_to(&mut self, owner: &StaticBody2D) {
        owner.emit_signal("started_moving", &[]);
    }
}
