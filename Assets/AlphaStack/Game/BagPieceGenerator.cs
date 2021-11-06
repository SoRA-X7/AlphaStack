using System;
using System.Collections.Generic;

namespace AlphaStack.Game {
    public class BagPieceGenerator : IPieceGenerator {
        private List<Piece> bag;
        private List<Piece> template;
        private Random rng;

        public BagPieceGenerator(List<Piece> bag) {
            rng = new Random();
            template = new List<Piece>(bag);
            this.bag = new List<Piece>(template);
        }

        public Piece Next() {
            var index = rng.Next(bag.Count);
            var taken = bag[index];
            bag.RemoveAt(index);
            if (bag.Count == 0) {
                bag.AddRange(template);
            }
            return taken;
        }
    }
}