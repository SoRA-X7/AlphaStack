using AlphaStack.Game.Routing;

namespace AlphaStack.Tbp {
    public class BotMoveResult {
        public Move move;
        public Route route;

        public BotMoveResult(Move move, Route route) {
            this.move = move;
            this.route = route;
        }
    }
}