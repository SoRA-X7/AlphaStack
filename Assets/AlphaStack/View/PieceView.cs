using System.Collections.Generic;
using AlphaStack.Game;
using UnityEngine;

namespace AlphaStack.View {
    public class PieceView : MonoBehaviour {
        [SerializeField] private CellView cellPrefab;
        private List<CellView> cells = new List<CellView>();
        private FallingPiece piece;

        public void Set(FallingPiece newPiece) {
            if (piece == newPiece) return;

            var p = newPiece.piece;
            if (piece.piece != p) {
                var respawn = piece.piece?.positions.Count != p?.positions?.Count;
                if (respawn) {
                    foreach (var tr in cells) {
                        Destroy(tr.gameObject);
                    }
                    cells.Clear();
                }

                var i = 0;
                foreach (var v in p.positions) {
                    var instance = respawn ? Instantiate(cellPrefab.gameObject, transform).GetComponent<CellView>() : cells[i++];
                    instance.Set(p.cell);
                    if (respawn) {
                        cells.Add(instance);
                    }
                }
            }

            piece = newPiece;
            for (var i = 0; i < cells.Count; i++) {
                var tr = cells[i].transform;
                var v2 = piece.RotatedPosition(i);
                tr.localPosition = new Vector3(v2.x, v2.y, 0);
            }
        }
    }
}