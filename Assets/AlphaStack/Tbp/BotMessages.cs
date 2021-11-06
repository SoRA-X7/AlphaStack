using System;
using System.Collections.Generic;

namespace AlphaStack.Tbp {
    [Serializable]
    public struct BotMessage {
        public string type;

        public const string KeyInfo = "info";
        public const string KeyReady = "ready";
        public const string KeySuggestion = "suggestion";
        public const string KeyError = "error";
    }

    [Serializable]
    public struct ErrorMessage {
        public string reason;
    }

    [Serializable]
    public struct InfoMessage {
        public string name;
        public string version;
        public string author;
        public List<string> features;
    }

    [Serializable]
    public struct SuggestionMessage {
        public List<Move> moves;
    }
}