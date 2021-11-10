using System;
using System.Text;
using UnityEngine;

namespace Prototyping {
    public class ConsoleReadProto : MonoBehaviour {
        private StringBuilder sb;

        private void Start() {
            sb = new StringBuilder();
        }

        private void Update() {
            while (Console.KeyAvailable) {
                var c = Console.ReadKey().KeyChar;
                sb.Append(c);
                if (c == '\n') {
                    Debug.Log(sb.ToString());
                    sb.Clear();
                }
            }
        }
    }
}