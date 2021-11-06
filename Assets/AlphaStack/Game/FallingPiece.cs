using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlphaStack.Game {
    public struct FallingPiece : IEquatable<FallingPiece> {
        public readonly Piece piece;
        public int x;
        public int y;
        public int r;
        public SpinStatus t;

        public FallingPiece(Piece piece, int x, int y, int r, SpinStatus t) {
            this.piece = piece;
            this.x = x;
            this.y = y;
            this.r = r;
            this.t = t;
        }

        public FallingPiece(Piece piece, int x, int y) : this() {
            this.piece = piece;
            this.x = x;
            this.y = y;
        }

        public int Size => piece.positions.Count;

        public IEnumerable<Vector2Int> Positions() {
            foreach (var pos in piece.positions) {
                yield return Piece.RotateFunctions[r](pos) + new Vector2Int(x, y);
            }
        }

        public Vector2Int RotatedPosition(int i) {
            return Piece.RotateFunctions[r](piece.positions[i]) + new Vector2Int(x, y);
        }

        public IEnumerable<FallingPiece> GetCanonicals() {
            var self = this;
            var c = piece.canonicals[self.r];
            return c.Select(pair =>
                new FallingPiece(self.piece, self.x + pair.Item2.x, self.y + pair.Item2.y, pair.Item1, self.t));
        }

        public bool Equals(FallingPiece other) {
            return piece == other.piece && x == other.x && y == other.y && r == other.r && t == other.t;
        }

        public override bool Equals(object obj) {
            return obj is FallingPiece other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (piece != null ? piece.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ x;
                hashCode = (hashCode * 397) ^ y;
                hashCode = (hashCode * 397) ^ r;
                hashCode = (hashCode * 397) ^ (int)t;
                return hashCode;
            }
        }

        public static bool operator ==(FallingPiece left, FallingPiece right) {
            return left.Equals(right);
        }

        public static bool operator !=(FallingPiece left, FallingPiece right) {
            return !left.Equals(right);
        }

        public override string ToString() {
            return $"{piece} at {x},{y},{r},{t}";
        }
    }
}