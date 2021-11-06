using System;
using AlphaStack.Game;
using UnityEngine;

namespace Prototyping {
    public class CellProto : MonoBehaviour {
        [SerializeField] private string cellType;
        private void Start() {
            var rend = GetComponent<Renderer>();
            rend.material = Cell.Registry[cellType].material;
        }
    }
}