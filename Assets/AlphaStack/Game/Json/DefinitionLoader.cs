using UnityEngine;

namespace AlphaStack.Game.Json {
    public static class DefinitionLoader {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Load() {
            Cell.Load();
            Piece.Load();
            RotationSystem.Load();
            AttackTable.Load();
        }
    }
}