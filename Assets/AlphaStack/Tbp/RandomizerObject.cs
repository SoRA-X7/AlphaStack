using System;
using System.Collections.Generic;
using System.Linq;
using AlphaStack.Game;

namespace AlphaStack.Tbp {
    public class RandomizerObject {
        public string type;
        public List<string> bag_state;
        public Dictionary<string, int> current_bag;
        public Dictionary<string, int> filled_bag;

        // ReSharper disable IdentifierTypo
        public bool ShouldSerializebag_state() => type == "seven_bag";
        public bool ShouldSerializecurrent_bag() => type == "general_bag";
        public bool ShouldSerializefilled_bag() => type == "general_bag";
        // ReSharper restore IdentifierTypo

        private RandomizerObject(string type) {
            this.type = type;
        }

        private RandomizerObject(BagPieceGenerator bag) : this("seven_bag") {
            bag_state = bag.bag.Select(p => p.name).ToList();
        }

        public static RandomizerObject Create(IPieceGenerator gen) {
            return gen switch {
                BagPieceGenerator bagged => new RandomizerObject(bagged),
                SyncedPieceGenerator synced => Create(synced.Internal),
                _ => throw new ArgumentOutOfRangeException(nameof(gen), gen, null)
            };
        }
    }
}