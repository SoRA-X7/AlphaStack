namespace AlphaStack.Game {
    public interface IPieceGenerator {
        Piece Next();
        IPieceGenerator Clone();
        string TbpType { get; }
    }
}