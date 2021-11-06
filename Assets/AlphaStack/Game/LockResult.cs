using System.Collections.Generic;

namespace AlphaStack.Game {
    public class LockResult {
        public List<int> clearedLines;
        public FallingPiece placement;
        public int attack;
        public int ren;
        public bool dead;
        public bool b2b;
        public bool allClear;
    }
}