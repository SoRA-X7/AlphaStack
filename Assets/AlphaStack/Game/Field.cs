using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AlphaStack.Game.Json;
using AlphaStack.Game.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AlphaStack.Game {
    public class Field {
        private Grid grid;
        public Piece HoldPiece { get; private set; }
        public readonly Queue<Piece> next;
        public bool BackToBack { get; private set; }
        public int Ren { get; private set; }
        public bool Dead { get; private set; }
        public int PieceCount { get; private set; }
        public int ClearedLinesCount { get; private set; }
        public int AttackSum { get; private set; }

        private Vector2Int size = new Vector2Int(10, 40);
        private Vector2Int spawnPosition = new Vector2Int(4, 19);
        public IPieceGenerator PieceGenerator { get; }
        private AttackTable attackTable;
        private DelaysConfig delays;
        private FallingPiece? currentPiece;
        private Stopwatch stopwatch;

        public FallingPiece? CurrentPiece {
            get => currentPiece;
            private set {
                currentPiece = value;
                Ghost = currentPiece.HasValue ? grid.SonicDrop(currentPiece.Value) : default(FallingPiece?);
            }
        }

        public FallingPiece? Ghost { get; private set; }

        public event Action<Piece> OnNewPiece;
        public event Action<LockResult> OnPieceLocked;

        public float PPS => PieceCount / (stopwatch?.ElapsedMilliseconds / 1000f) ?? 0f;
        public float LPM => ClearedLinesCount / (stopwatch?.ElapsedMilliseconds / 1000f / 60) ?? 0f;
        public float APM => AttackSum / (stopwatch?.ElapsedMilliseconds / 1000f / 60) ?? 0f;

        public Field(IPieceGenerator pieceGenerator, AttackTable attackTable, DelaysConfig delays) {
            grid = new Grid(size);
            this.PieceGenerator = pieceGenerator;
            this.attackTable = attackTable;
            this.delays = delays;
            next = new Queue<Piece>();
        }

        public void Init() {
            for (var i = 0; i < 5; i++) {
                FulfillNext();
            }
        }

        public void SpawnNext() {
            lock (next) {
                var piece = next.Dequeue();
                var spawned = new FallingPiece(piece, spawnPosition.x, spawnPosition.y);
                if (grid.Collides(spawned)) {
                    spawned = new FallingPiece(piece, spawnPosition.x, spawnPosition.y + 1);
                    if (grid.Collides(spawned)) {
                        Dead = true;
                        return;
                    }
                }

                CurrentPiece = spawned;
                FulfillNext();
            }

            if (stopwatch == null) {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
        }

        private void FulfillNext() {
            var newPiece = PieceGenerator.Next();
            next.Enqueue(newPiece);
            OnNewPiece?.Invoke(newPiece);
        }

        public bool MovePiece(Vector2Int direction) {
            if (CurrentPiece == null) return false;
            lock (this) {
                var moved = CurrentPiece.Value;
                moved.x += direction.x;
                moved.y += direction.y;
                if (grid.Collides(moved)) return false;
                CurrentPiece = moved;
                return true;
            }
        }

        public bool RotatePiece(bool clockwise) {
            lock (this) {
                if (CurrentPiece == null) return false;
                var moved = grid.Rotate(CurrentPiece.Value, clockwise);
                if (moved == null) return false;
                CurrentPiece = moved;
                return true;
            }
        }

        public async UniTask LockPiece(CancellationToken cancellationToken = default) {
            if (CurrentPiece == null) {
                throw new InvalidOperationException("No current piece available.");
            }

            lock (this) {
                CurrentPiece = Ghost!;

                grid.AddPiece(CurrentPiece!.Value);
                PieceCount += 1;
            }

            await AsyncUtils.Delay(delays.placement, cancellationToken);

            var placement = CurrentPiece!.Value;

            List<int> cleared;
            lock (this) {
                if (placement.Positions().All(v => v.y >= 20)) {
                    Dead = true;
                    OnPieceLocked?.Invoke(new LockResult {
                        placement = placement,
                        dead = true
                    });
                    return;
                }

                CurrentPiece = null;

                cleared = grid.ClearLines();
            }

            if (cleared.Any()) {
                Ren += 1;
            } else {
                Ren = 0;
            }

            var pc = grid.IsEmpty();
            var b2b = cleared.Count >= (placement.t == SpinStatus.None ? 4 : 1);
            var attack = pc ? attackTable.allClear : 0;

            if (!pc || attackTable.stackAllClear) {
                attack += (placement.t switch {
                    SpinStatus.None => attackTable.lineClears,
                    SpinStatus.Mini => attackTable.miniSpinClears,
                    SpinStatus.Full => attackTable.fullSpinClears
                })[cleared.Count];

                if (BackToBack && b2b) {
                    attack += attackTable.b2bBonus;
                }

                attack += attackTable.comboBonus[Mathf.Clamp(Ren, 0, attackTable.comboBonus.Count - 1)];
            }

            AttackSum += attack;
            
            OnPieceLocked?.Invoke(new LockResult {
                allClear = pc,
                attack = attack,
                ren = Ren,
                b2b = BackToBack && b2b,
                clearedLines = cleared,
                dead = false,
                placement = placement
            });

            if (cleared.Any()) {
                BackToBack = b2b;
                ClearedLinesCount += cleared.Count;
                await AsyncUtils.Delay(delays.lineClear, cancellationToken);
            }

            lock (this) {
                SpawnNext();
            }
        }

        public async UniTask Hold() {
            var tmp = HoldPiece;
            lock (this) {
                HoldPiece = CurrentPiece?.piece;
                CurrentPiece = null;
            }

            await AsyncUtils.Delay(delays.hold);
            lock (this) {
                if (tmp == null) {
                    SpawnNext();
                } else {
                    CurrentPiece = new FallingPiece(tmp, spawnPosition.x, spawnPosition.y);
                }
            }
        }

        public bool IsCurrentGrounded() {
            return CurrentPiece != null && CurrentPiece == Ghost;
        }

        public List<List<string>> ConvertToBoard() {
            var list = new List<List<string>>(size.y);
            for (var y = 0; y < size.y; y++) {
                list.Add(grid[y].Select(c => c?.name).ToList());
            }

            return list;
        }

        public Grid CopyGrid() => grid;
    }
}