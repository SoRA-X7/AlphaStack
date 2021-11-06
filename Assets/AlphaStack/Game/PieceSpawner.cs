namespace AlphaStack.Game {
    public class PieceSpawner {
        public FallingPiece Spawn(Piece piece) {
            return new FallingPiece(piece, 4, 19, 0, SpinStatus.None);
        }
    }
}