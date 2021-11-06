using System;
using System.Collections.Generic;
using System.Linq;
using AlphaStack.Game;
using AlphaStack.Game.Util;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace AlphaStack.View {
    public class FieldView : MonoBehaviour {
        [SerializeField] private PieceView piecePrefab;
        [SerializeField] private CellView cellPrefab;
        [SerializeField] private Transform origin;
        [SerializeField] private PieceView holdView;
        [SerializeField] private Transform nextOrigin;
        [SerializeField] private Transform edgesParent;

        [Header("UGUI")] [SerializeField] private TMP_Text ppsText;
        [SerializeField] private TMP_Text lpmText;
        [SerializeField] private TMP_Text apmText;

        [SerializeField] private RenUI renUI;
        [SerializeField] private WideTweenTextUI allClearUI;
        [SerializeField] private WideTweenTextUI backToBackUI;
        [SerializeField] private PlacementKindUI placementUI;

        [Header("Materials")] [SerializeField] private Material edgeMaterialNormal;
        [SerializeField] private Material edgeMaterialRed;
        

        public Player player;

        private Field field;

        private List<Renderer> edgeRenderers = new List<Renderer>();
        private List<PieceView> nextViews = new List<PieceView>();
        private Dictionary<Vector2Int, CellView> cells = new Dictionary<Vector2Int, CellView>();
        private ObjectPool<CellView> cellPool;

        private PieceView currentPieceView;
        // private PieceView ghostPieceView; //TODO

        private LockResult latestLock;
        private object placementLock = new object();

        private void Start() {
            cellPool = new ObjectPool<CellView>(
                () => Instantiate(cellPrefab.gameObject, origin).GetComponent<CellView>(),
                c => c.gameObject.SetActive(true),
                c => c.gameObject.SetActive(false),
                c => Destroy(c.gameObject));
            currentPieceView = Instantiate(piecePrefab.gameObject, origin).GetComponent<PieceView>();
            currentPieceView.gameObject.SetActive(false);
            edgeRenderers = edgesParent.GetComponentsInChildren<Renderer>().ToList();
        }

        private void Update() {
            foreach (var r in edgeRenderers) {
                r.material = player.HasError ? edgeMaterialRed : edgeMaterialNormal;
            }
            
            var f = player?.PlayerField;
            if (f == null) {
                field = null;
                return;
            }

            if (field != f) {
                field = f;
                field.OnPieceLocked += PieceLockedHandler;
            }

            lock (field) {
                if (field.HoldPiece != null) {
                    holdView.Set(new FallingPiece(field.HoldPiece, 0, 0));
                }
            }

            RenderNext();
            RenderGrid();
            RenderPieces();

            ppsText.SetTextFormat("<size=160%>{0}</size>, {1:N}/s", field.PieceCount, field.PPS);
            lpmText.SetTextFormat("<size=160%>{0}</size>, {1:N1}/m", field.ClearedLinesCount, field.LPM);
            apmText.SetTextFormat("<size=160%>{0}</size>, {1:N1}/m", field.AttackSum, field.APM);

            RenderPlacement();
        }

        private void PieceLockedHandler(LockResult obj) {
            if (obj.clearedLines?.Any() ?? false) {
                lock (placementLock) {
                    latestLock = obj;
                }
            }
        }

        private void RenderNext() {
            var nextCount = field.Lock(f => f.next.Count);

            if (nextCount != nextViews.Count) {
                foreach (var v in nextViews) {
                    Destroy(v.gameObject);
                }

                nextViews.Clear();

                for (var i = 0; i < nextCount; i++) {
                    var instance = Instantiate(piecePrefab.gameObject, nextOrigin);
                    if (i == 0) {
                        instance.transform.localPosition = Vector3.zero;
                        instance.transform.localScale = Vector3.one * 0.8f;
                    } else {
                        instance.transform.localPosition = new Vector3(0, -i * 2.5f - 0.5f, 0);
                        instance.transform.localScale = Vector3.one * 0.6f;
                    }

                    nextViews.Add(instance.GetComponent<PieceView>());
                }
            }

            lock (field) {
                foreach (var (piece, view) in field.next.Zip(nextViews, (piece, view) => (piece, view))) {
                    view.Set(new FallingPiece(piece, 0, 0));
                }
            }
        }

        private void RenderGrid() {
            var grid = field.CopyGrid();

            for (var y = 0; y < grid.size.y; y++) {
                var dataRow = grid[y];
                for (var x = 0; x < dataRow.Length; x++) {
                    var pos = new Vector2Int(x, y);
                    var dataCell = dataRow[x];
                    if (cells.TryGetValue(pos, out var c)) {
                        if (dataCell != null) {
                            c.Set(dataCell);
                        } else {
                            cellPool.Release(c);
                            cells.Remove(pos);
                        }
                    } else if (dataCell != null) {
                        var instance = cellPool.Get();
                        instance.Set(dataCell);
                        instance.transform.localPosition = new Vector3(x, y);
                        cells.Add(pos, instance);
                    }
                }
            }
        }

        private void RenderPieces() {
            if (field.CurrentPiece == null) {
                currentPieceView.gameObject.SetActive(false);
            } else {
                currentPieceView.gameObject.SetActive(true);
                currentPieceView.Set(field.CurrentPiece.Value);
            }

            // if (field.Ghost == null) {
            //     ghostPieceView.gameObject.SetActive(false);
            // } else {
            //     ghostPieceView.gameObject.SetActive(true);
            //     ghostPieceView.Set(field.Ghost.Value);
            // }
        }

        private void RenderPlacement() {
            lock (placementLock) {
                if (latestLock == null) return;

                if (latestLock.ren > 1) {
                    renUI.Show(latestLock.ren - 1);
                }

                if (latestLock.allClear) {
                    allClearUI.Show();
                }

                if (latestLock.b2b) {
                    backToBackUI.Show();
                }
                
                placementUI.Show(latestLock.clearedLines.Count, latestLock.placement.t, latestLock.placement.piece.ToString());

                latestLock = null;
            }
        }
    }
}