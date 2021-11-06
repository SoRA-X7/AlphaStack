using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaStack.Game.Json {
    [Serializable]
    public struct PieceDefinition {
        public string cellName;
        public List<Vector2Int> positions;
    }
}