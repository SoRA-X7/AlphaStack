using UnityEngine;

namespace AlphaStack.View {
    public class BotErrorPanelManager : MonoBehaviour {
        [SerializeField] private BotErrorPanelUI prefab;

        public void ShowError(string error) {
            var errorPanel = Instantiate(prefab.gameObject, transform).GetComponent<BotErrorPanelUI>();
            errorPanel.error = error;
        }
    }
}
