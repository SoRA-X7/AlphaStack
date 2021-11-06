namespace AlphaStack.Game.Routing {
    internal struct Node {
        public short parent;
        public byte cost;
        public Instruction instruction;
        public NativePiece placement;

        public Node(short parent, byte cost, Instruction instruction, NativePiece placement) {
            this.parent = parent;
            this.cost = cost;
            this.instruction = instruction;
            this.placement = placement;
        }
    }
}