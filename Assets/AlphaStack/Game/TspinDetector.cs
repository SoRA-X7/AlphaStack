using UnityEngine;

namespace AlphaStack.Game {
    public class TspinDetector : ISpinDetector {
        public void UpdateSpinStatus(Grid grid, ref FallingPiece piece, int kickIndex) {
            var rotate = Piece.RotateFunctions[piece.r];
            var pos = new Vector2Int(piece.x, piece.y);

            bool Check(Vector2Int offset) => grid.Occupied(rotate(offset) + pos);

            var miniChecks = 0;
            if (Check(new Vector2Int(-1, 1))) miniChecks++;
            if (Check(new Vector2Int(1, 1))) miniChecks++;

            var nonMiniChecks = 0;
            if (Check(new Vector2Int(-1, -1))) nonMiniChecks++;
            if (Check(new Vector2Int(1, -1))) nonMiniChecks++;

            if (miniChecks + nonMiniChecks >= 3) {
                if (kickIndex == 4 || miniChecks == 2) {
                    piece.t = SpinStatus.Full;
                } else {
                    piece.t = SpinStatus.Mini;
                }
            } else {
                piece.t = SpinStatus.None;
            }
        }
    }
}