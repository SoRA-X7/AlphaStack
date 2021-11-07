using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using AlphaStack.Game.Json;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AlphaStack.Game {
    public class Match : MonoBehaviour {
        public long TimeElapsed => stopwatch.ElapsedMilliseconds;
        private Stopwatch stopwatch;

        private List<Player> players;

        private GameRules rules;

        public bool Playing { get; private set; }

        private void Awake() {
            players = new List<Player>();
            stopwatch = new Stopwatch();
        }

        public void AddPlayer(Player pl) {
            players.Add(pl);
            pl.ID = players.Count;
            print($"Assigned ID {pl.ID}");
        }

        public void BeginMatch() {
            rules = new GameRules();
            rules.piecesInUse.AddRange(Piece.Registry.Values.Select(p => {
                // TODO
                foreach (var kv in RotationSystem.Registry) {
                    if (kv.Value.apply.Contains(p)) {
                        p.rotationSystem = kv.Value;
                        break;
                    }
                }

                if (p.name == "T") {
                    p.spinDetector = new TspinDetector();
                }

                return p;
            }));
            rules.pieceGenerator = new BagPieceGenerator(rules.piecesInUse);
            rules.attackTable = AttackTable.Registry["PPT2"];
            foreach (var player in players) {
                if (!player.SetRules(rules)) {
                    return;
                }
            }

            StartMatch();
        }

        private async UniTask StartMatch() {
            await UniTask.WhenAll(players.Select(p => UniTask.WaitUntil(() => p.Ready)));

            foreach (var player in players) {
                player.Prepare();
            }

            stopwatch.Restart();
            Playing = true;
            foreach (var player in players) {
                player.Start();
            }
        }

        public void StopMatch() {
            foreach (var player in players) {
                player.Stop();
            }
            Playing = false;
        }

        private void OnDisable() {
            StopMatch();
        }
    }
}