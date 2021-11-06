using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlphaStack.Game {
    public readonly struct Grid {
        private readonly List<Cell[]> grid;
        public readonly Vector2Int size;

        public Grid(Vector2Int size) {
            this.size = size;
            grid = new List<Cell[]>(size.y);
            for (int i = 0; i < size.y; i++) {
                grid.Add(new Cell[size.x]);
            }
        }

        public Cell[] this[int y] => grid[y];

        public bool IsEmpty() => grid.All(row => row.All(cell => cell == null));

        public bool Occupied(Vector2Int pos) {
            if (pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y) {
                return true;
            }

            return grid[pos.y][pos.x] != null;
        }

        public bool Collides(FallingPiece piece) {
            for (var i = 0; i < piece.Size; i++) {
                if (Occupied(piece.RotatedPosition(i))) return true;
            }

            return false;
        }

        public FallingPiece SonicDrop(FallingPiece piece) {
            while (true) {
                var prev = piece;
                piece.y -= 1;
                piece.t = SpinStatus.None;
                if (Collides(piece)) return prev;
            }
        }

        public FallingPiece? Strafe(FallingPiece piece, int offset) {
            piece.x += offset;
            return Collides(piece) ? (FallingPiece?)null : piece;
        }

        public FallingPiece? Rotate(FallingPiece piece, bool cw) {
            if (piece.piece.rotationSystem == null) return null;

            var rs = piece.piece.rotationSystem.offsets;
            var table = (piece.r, cw) switch {
                (0, true) => rs.ne,
                (0, false) => rs.nw,
                (1, true) => rs.es,
                (1, false) => rs.en,
                (2, true) => rs.sw,
                (2, false) => rs.se,
                (3, true) => rs.wn,
                (3, false) => rs.ws,
                _ => throw new Exception()
            };
            piece.r += cw ? 1 : 3;
            piece.r %= 4;
            for (var i = 0; i < table.Count; i++) {
                var moved = piece;
                var offset = table[i];
                moved.x += offset.x;
                moved.y += offset.y;
                if (!Collides(moved)) {
                    moved.piece.spinDetector?.UpdateSpinStatus(this, ref moved, i);
                    return moved;
                }
            }

            return null;
        }

        public void AddPiece(FallingPiece piece) {
            for (var i = 0; i < piece.Size; i++) {
                var pos = piece.RotatedPosition(i);
                grid[pos.y][pos.x] = piece.piece.cell;
            }
        }

        public List<int> ClearLines() {
            var clearedLines = new List<int>();
            for (var y = 0; y < grid.Count; y++) {
                if (grid[y].All(cell => cell != null)) {
                    clearedLines.Add(y);
                }
            }

            if (clearedLines.Count != 0) {
                var count = 0;
                for (var y = 0; y < grid.Count; y++) {
                    if (count < clearedLines.Count && y == clearedLines[count]) {
                        count += 1;
                    } else {
                        grid[y - count] = grid[y];
                    }
                }

                for (var i = 0; i < count; i++) {
                    grid[grid.Count - i - 1] = new Cell[size.x];
                }
            }

            return clearedLines;
        }
    }
}