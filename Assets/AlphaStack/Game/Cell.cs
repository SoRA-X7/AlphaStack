using System.Collections.Generic;
using System.IO;
using AlphaStack.Game.Json;
using UnityEngine;

namespace AlphaStack.Game {
    public class Cell {
        public readonly string name;
        public readonly Material material;

        public Cell(string name, Material material) {
            this.name = name;
            this.material = material;
        }

        public static Dictionary<string, Cell> Registry;
        
        public static void Load() {
            Registry = new Dictionary<string, Cell>();
            Debug.Log("Loading cell definitions...");
            var originalMat = Resources.Load<Material>("Cell");
            var folder = Path.Combine(Application.streamingAssetsPath, "Cell");
            var files = Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories);
            foreach (var path in files) {
                var text = File.ReadAllText(path);
                var name = Path.GetFileNameWithoutExtension(path);
                var data = JsonUtility.FromJson<CellDefinition>(text);
                var mat = new Material(originalMat);
                if (ColorUtility.TryParseHtmlString(data.colorCode, out var color)) {
                    mat.color = color;
                }
                Registry.Add(name, new Cell(name, mat));
                Debug.Log($"Loaded {name}");
            }
            Debug.Log($"Loaded {Registry.Count} cell definition(s)");
        }
    }
}