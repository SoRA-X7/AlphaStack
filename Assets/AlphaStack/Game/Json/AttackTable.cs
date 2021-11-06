using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AlphaStack.Game.Json {
    [Serializable]
    public class AttackTable {
        public List<int> lineClears;
        public List<int> fullSpinClears;
        public List<int> miniSpinClears;
        public List<int> comboBonus;
        public int b2bBonus;
        public int allClear;
        public bool stackAllClear;

        public static Dictionary<string, AttackTable> Registry;

        public static void Load() {
            Registry = new Dictionary<string, AttackTable>();
            Debug.Log("Loading attack tables...");
            var folder = Path.Combine(Application.streamingAssetsPath, "Attack");
            var files = Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories);
            foreach (var path in files) {
                var text = File.ReadAllText(path);
                var name = Path.GetFileNameWithoutExtension(path);
                var data = JsonUtility.FromJson<AttackTable>(text);
                Registry.Add(name, data);
                Debug.Log($"Loaded {name}");
            }
            Debug.Log($"Loaded {Registry.Count} attack tables(s)");
        }
    }
}