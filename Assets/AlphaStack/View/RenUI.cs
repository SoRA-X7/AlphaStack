using Cysharp.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AlphaStack.View {
    public class RenUI : MonoBehaviour {
        [SerializeField] private TMP_Text text;
        private Sequence seq;

        public void Start() {
            text.alpha = 0;
        }

        public void Show(int value) {
            seq.Complete();
            text.SetTextFormat("{0}<size=70%> REN</size>", value);
            seq = DOTween.Sequence()
                .Append(text.DOFade(1, 0.05f))
                .Append(text.DOFade(0, 1.5f));
            seq.Play();
        }
    }
}