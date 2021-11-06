// using System;
// using Unity.Collections;
// using Unity.Collections.LowLevel.Unsafe;
// using UnityEngine;
//
// namespace AlphaStack.Game.Routing {
//     internal struct NativeBoard : IDisposable {
//         private NativeBitArray array;
//         private Vector2Int size;
//         private NativeList<UnsafeList<Vector2Int>> pieceShapes;
//         private NativeList<UnsafeList<Vector2Int>> rotationRules;
//
//         public NativeBoard(Grid grid, Allocator allocator, NativeList<UnsafeList<Vector2Int>> pieceShapes) {
//             size = grid.size;
//             array = new NativeBitArray(size.x * size.y, allocator);
//             this.pieceShapes = pieceShapes;
//         }
//
//         public bool Occupied(Vector2Int pos) => 
//             pos.x < 0 || pos.x >= size.x ||
//             pos.y < 0 || pos.y >= size.y ||
//             array.IsSet(size.x * pos.y + pos.x);
//
//         public bool Collides(NativePiece piece) {
//             var result = false;
//             foreach (var pos in pieceShapes[piece.type]) {
//                 result |= Occupied(RotateFn(pos, piece.r) + piece.Pos);
//             }
//             return result;
//         }
//         
//         public NativePiece SonicDrop(NativePiece piece) {
//             while (!Collides(piece)) {
//                 piece.y -= 1;
//             }
//
//             piece.y += 1;
//             return piece;
//         }
//         
//         public NativePiece? Strafe(NativePiece piece, int offset) {
//             piece.x += offset;
//             return Collides(piece) ? default(NativePiece?) : piece;
//         }
//
//         public NativePiece? Rotate(NativePiece piece, bool cw) {
//             var rs = rotationRules[piece.type];
//             var table = (piece.r, cw) switch {
//                 (0, true) => rs.ne,
//                 (0, false) => rs.nw,
//                 (1, true) => rs.es,
//                 (1, false) => rs.en,
//                 (2, true) => rs.sw,
//                 (2, false) => rs.se,
//                 (3, true) => rs.wn,
//                 (3, false) => rs.ws,
//                 _ => throw new Exception()
//             };
//             piece.r += (byte) (cw ? 1 : 3);
//             piece.r %= 4;
//             for (var i = 0; i < table.Count; i++) {
//                 var moved = piece;
//                 var offset = table[i];
//                 moved.x += offset.x;
//                 moved.y += offset.y;
//                 if (!Collides(moved)) {
//                     moved.piece.spinDetector?.UpdateSpinStatus(this, ref moved, i);
//                     return moved;
//                 }
//             }
//
//             return null;
//         }
//
//         private static Vector2Int RotateFn(Vector2Int origin, int r) {
//             return (r % 4) switch {
//                 0 => origin,
//                 1 => new Vector2Int(origin.y, -origin.x),
//                 2 => new Vector2Int(-origin.x, -origin.y),
//                 3 => new Vector2Int(-origin.y, origin.x)
//             };
//         }
//
//         public void Dispose() {
//             array.Dispose();
//         }
//
//     }
// }