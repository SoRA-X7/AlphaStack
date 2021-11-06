using System;
using System.Collections.Generic;

namespace AlphaStack.Game.Json {
    [Serializable]
    public struct RotationSystemDefinition {
        public List<string> defaultApply;
        public RotationSystem.Offsets offsets;
    }
}