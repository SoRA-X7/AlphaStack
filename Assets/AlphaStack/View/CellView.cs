using AlphaStack.Game;
using UnityEngine;

namespace AlphaStack.View {
    public class CellView : MonoBehaviour {
        [SerializeField] private new Renderer renderer;

        private Cell cell;

        public void Set(Cell newCell) {
            if (cell == newCell) return;

            cell = newCell;
            renderer.material = cell.material;
        }
    }
}