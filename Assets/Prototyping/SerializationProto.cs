using System;
using System.Collections.Generic;
using AlphaStack.Tbp;
using UnityEngine;

namespace Prototyping {
    public class SerializationProto : MonoBehaviour {
        private void Start() {
            var s = new SuggestionMessage {
                moves = new List<Move> {
                    new Move {
                        location = new PieceLocation {
                            type = "T",
                            x = 3,
                            y = 2,
                            orientation = "north"
                        },
                        spin = "none"
                    }
                }
            };
            var serialized = JsonUtility.ToJson(s);
            Debug.Log(serialized);
            var deserialized = JsonUtility.FromJson<SuggestionMessage>(serialized);
        }
    }
}