using System;
using AlphaStack.Game;

namespace AlphaStack.Tbp {
    [Serializable]
    public struct Move {
        public PieceLocation location;
        public string spin;

        public FallingPiece ToFallingPiece() {
            return new FallingPiece(
                Piece.Registry[location.type],
                location.x,
                location.y,
                location.orientation switch {
                    "north" => 0,
                    "east" => 1,
                    "south" => 2,
                    "west" => 3,
                    _ => throw new InvalidCastException()
                },
                spin switch {
                    "none" => SpinStatus.None,
                    "mini" => SpinStatus.Mini,
                    "full" => SpinStatus.Full,
                    _ => throw new InvalidCastException()
                }
            );
        }
    }
}