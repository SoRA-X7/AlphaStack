namespace AlphaStack.Game {
    public interface ISpinDetector {
        void UpdateSpinStatus(Grid grid, ref FallingPiece piece, int kickIndex);
    }
}