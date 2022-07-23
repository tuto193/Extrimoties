use gdnative::api::tween::{EaseType, TransitionType};
use gdnative::api::*;
use gdnative::export::Export;
use gdnative::export::hint::{EnumHint, IntHint};
// use gdnative::object::ownership;
use gdnative::prelude::*;

use crate::grid::Grid;

#[derive(Copy, Clone, PartialEq)]
pub enum CellType {
    Empty = -1,
    Wall,
    Box,
    Life,
    Goal,
}

impl CellType {
    pub fn from_i64(n: i64) -> Result<Self, &'static str> {
        match n {
            -1 => Ok(CellType::Empty),
            0 => Ok(CellType::Wall),
            1 => Ok(CellType::Box),
            2 => Ok(CellType::Life),
            3 => Ok(CellType::Goal),
            _ => Err("Expected integer in [-1, 3]"),
        }
    }
    // fn to_str(self) -> String {
    //     match self {
    //         CellType::Empty => "empty".to_string(),
    //         CellType::Wall=> "wall".to_string(),
    //         CellType::Box => "box".to_string(),
    //         CellType::Life => "life".to_string(),
    //         CellType::Goal => "goal".to_string(),
    //     }
    // }
}

impl FromVariant for CellType {
    fn from_variant(variant: &Variant) -> Result<Self, FromVariantError> {
        let result = i64::from_variant(variant)?;
        match result {
            -1 => Ok(CellType::Empty),
            0 => Ok(CellType::Wall),
            1 => Ok(CellType::Box),
            2 => Ok(CellType::Life),
            3 => Ok(CellType::Goal),
            _ => Err(FromVariantError::UnknownEnumVariant {
                variant: "i64".to_owned(),
                expected: &["-1", "0", "1", "2", "3"],
             }),
        }
    }
}

impl Export for CellType {
    type Hint = IntHint<u32>;

    fn export_info(_hint: Option<Self::Hint>) -> ExportInfo {
        Self::Hint::Enum(EnumHint::new(vec![
            "Empty".to_owned(),
            "Wall".to_owned(),
            "Box".to_owned(),
            "Life".to_owned(),
            "Goal".to_owned(),
        ]))
        .export_info()
    }
}

impl ToVariant for CellType {
    fn to_variant(&self) -> Variant {
        match self {
            CellType::Empty => { (-1).to_variant() },
            CellType::Wall => { 0.to_variant() },
            CellType::Box => { 1.to_variant() },
            CellType::Life => { 2.to_variant() },
            CellType::Goal => { 3.to_variant() },
        }
    }
}

#[derive(NativeClass)]
#[inherit(StaticBody2D)]
#[register_with(Self::register_signals)]
pub struct GridPiece {
    grid: Option<Instance<Grid>>,
    tween: Option<Ref<Tween>>,
    #[property(default = 0.25)]
    time_animation: f64,
    pub cell_type: CellType,
}

#[methods]
impl GridPiece {
    fn register_signals(builder: &ClassBuilder<Self>) {
        builder.signal("started_moving").done();
        builder.signal("finished_moving").done();
        builder.signal("fall_in_hole").done();
        builder.signal("goal_reached").done();
    }

    fn new(_owner: &StaticBody2D) -> Self {
        Self {
            grid: None,
            tween: None,
            time_animation: 0.25,
            cell_type: CellType::Empty,
        }
    }

    #[export]
    fn _ready(&mut self, owner: TRef<StaticBody2D>) {
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
        tween.connect("tween_completed", owner, "on_tween_tween_completed", VariantArray::new_shared(), 0);
        self.tween = Some(tween.claim());
    }

    pub fn move_to(&mut self, owner: TRef<StaticBody2D>, new_pos: Vector2) {
        owner.emit_signal("started_moving", &[]);
        owner.set_process(false);
        let t = unsafe { self.tween.unwrap().assume_safe() };
        let _ = t.interpolate_property(
            owner,
            "position",
            owner.position(),
            new_pos,
            self.time_animation,
            i64::from(TransitionType::SINE),
            i64::from(EaseType::OUT),
            0.0,
        );
        let _ = t.start();
    }

    fn on_tween_tween_completed(&self, owner: TRef<StaticBody2D>) {
        owner.set_process(true);
        owner.emit_signal("finished_moving", &[]);
    }

    fn handle_hole(&self, owner: TRef<StaticBody2D>, _hole: TInstance<GridPiece>) -> bool {
        owner.emit_signal("fall_in_hole", &[]);
        return true;
    }
}
