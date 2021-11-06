using System;

namespace AlphaStack.Tbp {
    [Serializable]
    public struct PieceLocation {
        public string type;
        public int x;
        public int y;
        public string orientation;
    }
}