using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AlphaStack.View {
    public class WideTweenTextUI : MonoBehaviour {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float target = 30f;
        private Sequence seq;

        public void Start() {
            text.alpha = 0;
        }

        public void Show() {
            seq.Complete();
            text.characterSpacing = 0;
            seq = DOTween.Sequence()
                .Append(DOTween.Sequence()
                    .Append(text.DOFade(1, 0.2f))
                    .Append(text.DOFade(0, duration - 0.2f)))
                .Join(DOTween.To(
                    () => text.characterSpacing,
                    x => text.characterSpacing = x,
                    target, duration).SetEase(Ease.Linear));
            seq.Play();
        }
    }
}