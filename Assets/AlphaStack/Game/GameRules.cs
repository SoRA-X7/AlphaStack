using System.Collections.Generic;
using AlphaStack.Game.Json;

namespace AlphaStack.Game {
    public class GameRules {
        public List<Piece> piecesInUse = new List<Piece>();
        public AttackTable attackTable;
    }
}