using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace AlphaStack.Game.Routing {
    public sealed class MoveFinder {
        private bool working;
        private readonly List<Node> tree;
        private readonly List<NodeRef> next;
        private readonly Dictionary<FallingPiece, NodeRef> locked;
        private readonly HashSet<FallingPiece> passed;

        public MoveFinder() {
            tree = new List<Node>(200);
            next = new List<NodeRef>(200);
            locked = new Dictionary<FallingPiece, NodeRef>(4000);
            passed = new HashSet<FallingPiece>();
        }

        public async UniTask FindValidPlacements(Grid grid, Piece piece, Piece hold, PieceSpawner spawner) {
            if (working) throw new Exception();

            working = true;

            tree.Clear();
            next.Clear();
            locked.Clear();
            passed.Clear();

            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                Work(grid, spawner.Spawn(piece));
                if (hold != piece) {
                    Work(grid, spawner.Spawn(hold));
                }
            } else {
                await Task.Run(() => Work(grid, spawner.Spawn(piece)));
                if (hold != piece) {
                    await Task.Run(() => Work(grid, spawner.Spawn(hold)));
                }
            }

            working = false;
        }

        private void Work(in Grid grid, FallingPiece piece) {
            var root = new Node(-1, 0, Instruction.None, piece);
            tree.Add(root);
            next.Add(new NodeRef(root, tree.Count - 1));

            while (next.Any()) {
                next.Sort();
                var current = next[0];
                next.RemoveAtSwapBack(0);

                var node = tree[current.node];

                Apply(current.node, grid.Strafe(node.placement, -1), Instruction.Left);
                Apply(current.node, grid.Strafe(node.placement, 1), Instruction.Right);
                Apply(current.node, grid.Rotate(node.placement, true), Instruction.Cw);
                Apply(current.node, grid.Rotate(node.placement, false), Instruction.Ccw);

                var dropped = grid.SonicDrop(node.placement);
                if (dropped.y != piece.y) {
                    Apply(current.node, dropped, Instruction.SonicDrop);
                }

                if (!locked.ContainsKey(dropped)) {
                    locked.Add(dropped, new NodeRef(node, current.node));
                }
            }

            void Apply(int parentIndex, FallingPiece? place, Instruction inst) {
                if (place is null) return;
                var parent = tree[parentIndex];
                var result = place.Value;

                int t;

                if (inst == Instruction.SonicDrop) {
                    t = 2 * (parent.placement.y - result.y);
                } else {
                    t = 1;
                }

                if (parent.instruction == inst) {
                    t += 1;
                }

                var node = new Node((short)parentIndex, (byte)(parent.cost + t), inst, result);

                if (!passed.Contains(result)) {
                    next.Add(new NodeRef(node, tree.Count));
                    tree.Add(node);
                    passed.Add(result);
                }
            }
        }

        public Route? BuildRoute(FallingPiece to, bool holdUsed) {
            if (working) throw new Exception();

            if (!locked.TryGetValue(to, out var leafRef)) return null;

            var instructions = new List<Instruction>();
            var leaf = leafRef.node;
            while (leaf >= 0) {
                var node = tree[leaf];
                instructions.Add(node.instruction);
                leaf = node.parent;
            }

            instructions.Reverse();

            var route = new Route {
                hold = holdUsed,
                instructions = instructions.AsReadOnly(),
                result = to,
                cost = locked[to].cost
            };

            return route;
        }

        private struct Node {
            public short parent;
            public byte cost;
            public Instruction instruction;
            public FallingPiece placement;

            public Node(short parent, byte cost, Instruction instruction, FallingPiece placement) {
                this.parent = parent;
                this.cost = cost;
                this.instruction = instruction;
                this.placement = placement;
            }
        }

        private struct NodeRef : IComparable<NodeRef> {
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
}