using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AlphaStack.Game.Json;
using UnityEngine;

namespace AlphaStack.Game {
    public class Piece {
        public readonly string name;
        public readonly Cell cell;
        public readonly List<Vector2Int> positions;
        public RotationSystem rotationSystem;
        public ISpinDetector spinDetector;

        public readonly List<(int, Vector2Int)>[] canonicals;

        public static Dictionary<string, Piece> Registry;

        public static readonly Func<Vector2Int, Vector2Int>[] RotateFunctions = {
            v => v,
            v => new Vector2Int(v.y, -v.x),
            v => new Vector2Int(-v.x, -v.y),
            v => new Vector2Int(-v.y, v.x)
        };

        public Piece(string name, Cell cell, List<Vector2Int> positions) {
            this.name = name;
            this.cell = cell;
            this.positions = positions;
            
            var rotated = RotateFunctions.Select(fn => positions.Select(fn).ToList()).ToList();
            
            var xRange = Math.Abs(Math.Max(-positions.Min(p => p.x), positions.Max(p => p.x))) + 1;
            var yRange = Math.Abs(Math.Max(-positions.Min(p => p.y), positions.Max(p => p.y))) + 1;
            canonicals = new List<(int, Vector2Int)>[4];
            for (var i = 0; i < 4; i++) {
                canonicals[i] = new List<(int, Vector2Int)>();
                for (var x = -xRange; x <= xRange; x++) {
                    for (var y = -yRange; y <= yRange; y++) {
                        var offset = new Vector2Int(x, y);
                        for (var s = 0; s < 4; s++) {
                            if (!rotated[i].All(b => rotated[s].Contains(b - offset))) continue;
                            canonicals[i].Add((s, offset));
                        }
                    }
                }
            }
        }

        public Piece(Piece from) {
            name = from.name;
            cell = from.cell;
            positions = new List<Vector2Int>(from.positions);
            rotationSystem = from.rotationSystem;
            spinDetector = from.spinDetector;
            canonicals = from.canonicals;
        }

        public override string ToString() {
            return name;
        }

        public static void Load() {
            Registry = new Dictionary<string, Piece>();
            Debug.Log("Loading piece definitions...");
            var folder = Path.Combine(Application.streamingAssetsPath, "Piece");
            var files = Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories);
            foreach (var path in files) {
                var text = File.ReadAllText(path);
                var name = Path.GetFileNameWithoutExtension(path);
                var data = JsonUtility.FromJson<PieceDefinition>(text);
                var piece = new Piece(name, Cell.Registry[data.cellName], data.positions);
                Registry.Add(name, piece);
                Debug.Log($"Loaded {name}");
            }
            Debug.Log($"Loaded {Registry.Count} piece definition(s)");
        }
    }
}