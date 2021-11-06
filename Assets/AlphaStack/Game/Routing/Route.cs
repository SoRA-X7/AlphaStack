using System.Collections.Generic;

namespace AlphaStack.Game.Routing {
    public struct Route {
        public bool hold;
        public IReadOnlyList<Instruction> instructions;
        public FallingPiece result;
        public int cost;

        public override string ToString() {
            return $"{result} {cost} <- {(hold ? "HOLD " : "")}{string.Join(" ", instructions)}";
        }
    }
}