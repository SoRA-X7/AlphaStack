using System;
using System.Collections.Generic;

namespace AlphaStack.Game {
    public class BagPieceGenerator : IPieceGenerator {
        public List<Piece> bag;
        public List<Piece> template;
        private Random rng;
        
        private BagPieceGenerator(List<Piece> bag, List<Piece> template) {
            this.template = new List<Piece>(bag);
            this.bag = new List<Piece>(template);
            this.rng = new Random();
        }

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

        public IPieceGenerator Clone() {
            return new BagPieceGenerator(bag, template);
        }

        public string TbpType => "seven_bag";
    }
}