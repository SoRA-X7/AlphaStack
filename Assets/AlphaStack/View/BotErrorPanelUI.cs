using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AlphaStack.View {
    public class BotErrorPanelUI : MonoBehaviour {
        public string error;
        [SerializeField] private RectTransform panel;
        [SerializeField] private TMP_Text text;
        private RectTransform rectTransform;

        private void Start() {
            text.text = error;
            Debug.Log(error);

            rectTransform = GetComponent<RectTransform>();
            DOTween.Sequence()
                .Append(panel.DOLocalMoveY(rectTransform.sizeDelta.y, 0.5f).From())
                .Insert(4f, rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 0), 0.5f))
                .Join(panel.DOLocalMoveX(-rectTransform.sizeDelta.x, 0.5f))
                .OnComplete(() => Destroy(gameObject))
                .Play();
        }
    }
}
