// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
//
// namespace AlphaStack.Game.Routing {
//     [BurstCompile]
//     internal struct MoveFindJob : IJob {
//         public NativeList<Node> tree;
//         public NativeList<NodeRef> next;
//         public NativeHashMap<NativePiece, NodeRef> locked;
//         public NativeHashSet<NativePiece> passed;
//         
//         public void Execute() {
//             
//         }
//         
//         private void Work(in NativeBoard board, FallingPiece piece) {
//             var root = new Node(-1, 0, Instruction.None, piece);
//             tree.Add(root);
//             next.Add(new NodeRef(root, tree.Length - 1));
//
//             while (next.Length > 0) {
//                 next.Sort();
//                 var current = next[0];
//                 next.RemoveAtSwapBack(0);
//
//                 var node = tree[current.node];
//
//                 Apply(current.node, board.Strafe(node.placement, -1), Instruction.Left);
//                 Apply(current.node, board.Strafe(node.placement, 1), Instruction.Right);
//                 Apply(current.node, board.Rotate(node.placement, true), Instruction.Cw);
//                 Apply(current.node, board.Rotate(node.placement, false), Instruction.Ccw);
//
//                 var dropped = grid.SonicDrop(node.placement);
//                 if (dropped.y != piece.y) {
//                     Apply(current.node, dropped, Instruction.SonicDrop);
//                 }
//
//                 if (!locked.ContainsKey(dropped)) {
//                     locked.Add(dropped, new NodeRef(node, current.node));
//                 }
//             }
//
//             void Apply(int parentIndex, FallingPiece? place, Instruction inst) {
//                 if (place is null) return;
//                 var parent = tree[parentIndex];
//                 var result = place.Value;
//
//                 int t;
//
//                 if (inst == Instruction.SonicDrop) {
//                     t = 2 * (parent.placement.y - result.y);
//                 } else {
//                     t = 1;
//                 }
//
//                 if (parent.instruction == inst) {
//                     t += 1;
//                 }
//
//                 var node = new Node((short)parentIndex, (byte)(parent.cost + t), inst, result);
//
//                 if (!passed.Contains(result)) {
//                     next.Add(new NodeRef(node, tree.Count));
//                     tree.Add(node);
//                     passed.Add(result);
//                 }
//             }
//         }
//     }
// }