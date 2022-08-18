using Godot;
using System;
using static GridTraits;
using static GridPiece;

public class Grid : TileMap {
	private Godot.Collections.Array able_to_fall_pieces = new Godot.Collections.Array {CellType.Life, CellType.Box};
	private PackedScene grid_piece = GD.Load<PackedScene>("res://Scenes/Blocks/GridPiece.tscn");
	public override void _Ready() {
		foreach(Vector2 cell_pos in GetUsedCells()) {
			// Box, Goal, Hole, Wall
			int cell_type = GetCellv(cell_pos);
			String tile_name = TileSet.TileGetName(cell_type).ToUpper();
			// GD.Print($"Cell {cell_pos} is of TileSet type {tile_name}");
			// Need to instance the actual scenes, add them as children, set the cell to the actual type
			// Just initializing. Overwrite this value
			CellType expected_type = GridPiece.CellTypeFromString(tile_name);
			GridPiece gp = grid_piece.Instance<GridPiece>();
			gp.ReInitialize(expected_type);
			gp.Position = MapToWorld(cell_pos) * Scale;
			// gp.ReInitialize(expected_type);
			gp.InitialPosition = gp.Position;
			AddChild(gp);
			// gp.ReInitialize(expected_type);
			// Initialize the TileMap stuff
			SetCellv(cell_pos, (int) gp.Cell_Type);
			// Make sure that all CellTypes that can fall, are added to this
			if (able_to_fall_pieces.Contains(gp.Cell_Type) ) {
				gp.Connect(nameof(FallInHole), this, nameof(OnGridPieceFallInHole), new Godot.Collections.Array {gp});
			}
		}
	}

	private GridPiece GetCellContent(Vector2 coordinates) {
		foreach (GridPiece gp in GetChildren()) {
			if (WorldToMap(gp.Position) == coordinates) {
				return gp;
			}
		}
		return null;
	}

	public Vector2 MovePieceTowards(GridPiece piece, Vector2 direction) {
		Vector2 cell_map_start = WorldToMap(piece.Position);
		Vector2 cell_map_target = cell_map_start + direction;
		CellType target_cell_type = (CellType) GetCellv(cell_map_target);
		GridPiece object_piece = GetCellContent(cell_map_target);

		switch (target_cell_type) {
			case CellType.Box:
				// GD.Print("OMG, a Box!");
				if (piece.Cell_Type == CellType.Box) {
					GD.Print("Cell {} already contains a box. Cannot push two at a time", cell_map_target);
					return piece.Position;
				}
				Vector2 object_world_start = object_piece.Position;
				Vector2 object_new_pos = MovePieceTowards(
					object_piece,
					direction
				);
				if (object_world_start == object_new_pos) {
					return piece.Position;
				}
				Vector2 new_pos = UpdateGridPositions(
					piece,
					cell_map_start,
					cell_map_target
				);
				piece.MoveTo(new_pos);
				return new_pos;
			case CellType.Wall:
				GD.Print($"Cell {object_piece} contains a wall", cell_map_target);
				return piece.Position;
			case CellType.Empty:
				Vector2 new_pos2 = UpdateGridPositions(
					piece,
					cell_map_start,
					cell_map_target
				);
				piece.MoveTo(new_pos2);
				return new_pos2;
			case CellType.Hole:
				GD.Print("aAaAaAaAaAaA");
				Vector2 new_pos3 = UpdateGridPositions(
					piece,
					cell_map_start,
					cell_map_target
				);
				piece.MoveTo(new_pos3, target_cell_type);
				// object_piece).StepIntoCheck(piece);
				piece.EnterHole();
				return new_pos3;
			case CellType.Goal:
				// GD.Print("Almost won!");
				Vector2 new_pos4 = UpdateGridPositions(
					piece,
					cell_map_start,
					cell_map_target
				);
				piece.MoveTo(new_pos4, target_cell_type);
				// ((Goal) object_piece).StepIntoCheck(piece);
				return new_pos4;
			default:
				GD.Print("Whatever the hell this is");
				Vector2 new_pos5 = UpdateGridPositions(
					piece,
					cell_map_start,
					cell_map_target
				);
				piece.MoveTo(new_pos5);
				return new_pos5;
		}
	}

	private Vector2 UpdateGridPositions(
		GridPiece piece,
		Vector2 cell_map_start,
		Vector2 cell_map_target
	) {
		SetCellv(cell_map_target, (int) piece.Cell_Type);
		SetCellv(cell_map_start, (int) piece.IsStandingOn);
		return MapToWorld(cell_map_target) + CellSize / 2;
	}

	// ReSpawn the Grid piece
	public void OnGridPieceFallInHole(GridPiece gp) {
		Vector2 _init_pos = UpdateGridPositions(gp, WorldToMap(gp.Position), WorldToMap(gp.InitialPosition));
		gp.MoveTo(gp.InitialPosition);
	}
}
