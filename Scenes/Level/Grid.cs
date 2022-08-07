using Godot;
using System;
using static GridTraits;

public class Grid : TileMap {
	public override void _Ready() {
		foreach (GridPiece child in GetChildren()) {
			SetCellv(WorldToMap(child.Position), ((int) child.Cell_Type));
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
				GD.Print("Cell {} contains a wall", cell_map_target);
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
				Vector2 new_pos3 = UpdateGridPositions(
					piece,
					cell_map_start,
					cell_map_target
				);
				piece.MoveTo(new_pos3, target_cell_type);
				((Hole) object_piece).StepIntoCheck(piece);
				return new_pos3;
			case CellType.Goal:
				Vector2 new_pos4 = UpdateGridPositions(
					piece,
					cell_map_start,
					cell_map_target
				);
				piece.MoveTo(new_pos4, target_cell_type);
				((Goal) object_piece).StepIntoCheck(piece);
				return new_pos4;
			default:
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
}
