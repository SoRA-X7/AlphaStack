using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AlphaStack.Game.Json;
using UnityEngine;

namespace AlphaStack.Game {
    public class RotationSystem {
        public List<Piece> apply;
        public Offsets offsets;

        public static Dictionary<string, RotationSystem> Registry;

        public RotationSystem(List<Piece> apply, Offsets offsets) {
            this.apply = apply;
            this.offsets = offsets;
        }

        [Serializable]
        public struct Offsets {
            public List<Vector2Int> ne;
            public List<Vector2Int> nw;
            public List<Vector2Int> es;
            public List<Vector2Int> en;
            public List<Vector2Int> sw;
            public List<Vector2Int> se;
            public List<Vector2Int> wn;
            public List<Vector2Int> ws;
        }

        public static void Load() {
            Registry = new Dictionary<string, RotationSystem>();
            Debug.Log("Loading rotation system definitions...");
            var folder = Path.Combine(Application.streamingAssetsPath, "Rotation");
            var files = Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories);
            foreach (var path in files) {
                var text = File.ReadAllText(path);
                var name = Path.GetFileNameWithoutExtension(path);
                var data = JsonUtility.FromJson<RotationSystemDefinition>(text);
                Registry.Add(name, new RotationSystem(
                        data.defaultApply.Select(s => Piece.Registry.TryGetValue(s, out var piece) ? piece : null)
                            .Where(p => p != null).ToList(),
                        data.offsets));
                Debug.Log($"Loaded {name}");
            }
            Debug.Log($"Loaded {Registry.Count} rotation system definition(s)");
        }
    }
}