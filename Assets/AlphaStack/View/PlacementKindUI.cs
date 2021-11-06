using System;
using AlphaStack.Game;
using Cysharp.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AlphaStack.View {
    public class PlacementKindUI : MonoBehaviour {
        [SerializeField] private TMP_Text spinText;
        [SerializeField] private TMP_Text kindText;
        [SerializeField] private WideTweenTextUI kindTweener;

        private void Start() {
            spinText.alpha = 0;
            kindText.alpha = 0;
        }

        public void Show(int clearedLines, SpinStatus status, string pieceName) {
            if (status != SpinStatus.None) {
                spinText.SetTextFormat(
                    status == SpinStatus.Mini ? "MINI <b>{0}-Spin</b>" : "<b>{0}-Spin</b>", pieceName);

                spinText.alpha = 1;
                spinText.DOFade(0, 2f);
            }

            if (clearedLines > 0) {
                kindText.SetText(clearedLines switch {
                    1 => "SINGLE",
                    2 => "DOUBLE",
                    3 => "TRIPLE",
                    4 => "QUAD"
                });
                kindTweener.Show();
            }
        }
    }
}