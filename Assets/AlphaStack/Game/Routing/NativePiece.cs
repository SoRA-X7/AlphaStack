using System;
using UnityEngine;

namespace AlphaStack.Game.Routing {
    internal struct NativePiece : IEquatable<NativePiece> {
        public int type;
        public int x;
        public int y;
        public byte r;
        public byte s;
        public Vector2Int Pos => new Vector2Int(x, y);

        public bool Equals(NativePiece other) {
            return type == other.type && x == other.x && y == other.y && r == other.r && s == other.s;
        }

        public override bool Equals(object obj) {
            return obj is NativePiece other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = type;
                hashCode = (hashCode * 397) ^ x;
                hashCode = (hashCode * 397) ^ y;
                hashCode = (hashCode * 397) ^ r.GetHashCode();
                hashCode = (hashCode * 397) ^ s.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(NativePiece left, NativePiece right) {
            return left.Equals(right);
        }

        public static bool operator !=(NativePiece left, NativePiece right) {
            return !left.Equals(right);
        }
    }
}