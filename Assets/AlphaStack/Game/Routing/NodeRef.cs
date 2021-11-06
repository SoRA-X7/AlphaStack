using System;

namespace AlphaStack.Game.Routing {
    internal struct NodeRef : IComparable<NodeRef> {
        public short node;
        public byte cost;

        public NodeRef(Node node, int index) {
            this.node = (short)index;
            this.cost = node.cost;
        }

        public int CompareTo(NodeRef other) {
            return cost.CompareTo(other.cost);
        }
    }
}