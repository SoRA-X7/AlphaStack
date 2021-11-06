using System;
using AlphaStack.Game;
using UnityEngine;
using UnityEngine.UI;

namespace AlphaStack.View {
    public class MatchControlPanelUI : MonoBehaviour {
        [SerializeField] private Match match;
        
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private Button stepButton;

        private void Update() {
            playButton.gameObject.SetActive(!match.Playing);
            pauseButton.gameObject.SetActive(match.Playing);
            stopButton.interactable = match.Playing;
            stepButton.interactable = match.Playing;
        }
    }
}