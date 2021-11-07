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

        public SyncedPieceGenerator Share() {
            return new SyncedPieceGenerator(shared);
        }

        public Piece Next() {
            lock (shared) {
                return shared.Get(current++);
            }
        }

        public IPieceGenerator Clone() {
            return (IPieceGenerator)MemberwiseClone();
        }

        public string TbpType => shared.origin.TbpType;
        public IPieceGenerator Internal => shared.origin;

        private class Shared {
            public readonly IPieceGenerator origin;
            public readonly List<Piece> generated;

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