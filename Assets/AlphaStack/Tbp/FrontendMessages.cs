using System;
using System.Linq;
using System.Collections.Generic;
using AlphaStack.Game;

namespace AlphaStack.Tbp {
    [Serializable]
    public class FrontendMessage {
        public string type;
        public FrontendMessage(string type) {
            this.type = type;
        }

        public const string KeyRules = "rules";
        public const string KeyStart = "start";
        public const string KeySuggest = "suggest";
        public const string KeyPlay = "play";
        public const string KeyNewPiece = "new_piece";
        public const string KeyStop = "stop";
        public const string KeyQuit = "quit";
    }

    [Serializable]
    public class RulesMessage : FrontendMessage {
        public string randomizer;
        public RulesMessage() : base(KeyRules) { }
    }

    [Serializable]
    public class StartMessage : FrontendMessage {
        public string hold;
        public List<string> queue;
        public int combo;
        public bool back_to_back;
        public List<List<string>> board;
        public RandomizerObject randomizer;

        public StartMessage(Field field) : base(KeyStart) {
            hold = field.HoldPiece?.name;
            queue = new List<string>(field.next.Select(p => p?.name));
            combo = field.Ren;
            back_to_back = field.BackToBack;
            board = field.ConvertToBoard();
            randomizer = RandomizerObject.Create(field.PieceGenerator);
        }
    }

    [Serializable]
    public class NewPieceMessage : FrontendMessage {
        public string piece;
        public NewPieceMessage(Piece piece) : base(KeyNewPiece) {
            this.piece = piece.name;
        }
    }

    [Serializable]
    public class PlayMessage : FrontendMessage {
        public Move move;
        public PlayMessage(Move move) : base(KeyPlay) {
            this.move = move;
        }
    }
}