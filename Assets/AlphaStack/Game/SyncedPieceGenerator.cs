using System.Collections.Generic;

namespace AlphaStack.Game {
    public class SyncedPieceGenerator : IPieceGenerator {
        private readonly Shared shared;
        private int current;

        public SyncedPieceGenerator(IPieceGenerator generator) {
            shared = new Shared(generator);
        }

        private SyncedPieceGenerator(Shared shared) {
            this.shared = shared;
        }

        public SyncedPieceGenerator Clone() {
            return new SyncedPieceGenerator(shared);
        }

        public Piece Next() {
            lock (shared) {
                return shared.Get(current++);
            }
        }

        private class Shared {
            private readonly IPieceGenerator origin;
            private readonly List<Piece> generated;

            public Shared(IPieceGenerator origin) {
                this.origin = origin;
                generated = new List<Piece>();
            }

            public Piece Get(int i) {
                while (generated.Count < i) {
                    generated.Add(origin.Next());
                }

                return generated[i];
            }
        }
    }
}