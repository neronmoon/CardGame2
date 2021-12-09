using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Sources.Unity {
    public class LevelAnnounceView : MonoBehaviour {
        public TextMeshProUGUI LevelNameText;
        public CanvasGroup CanvasGroup;

        private float targetPosition;
        private RectTransform rt;

        private void Start() {
            rt = GetComponent<RectTransform>();
            targetPosition = rt.anchoredPosition.y;
            Reset();
        }

        private void Reset() {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.rect.height);
            CanvasGroup.alpha = 1f;
        }

        public void AnnounceLevel(string levelName) {
            LevelNameText.text = levelName;

            DOTween.Sequence()
                   .Append(rt.DOAnchorPosY(targetPosition, 0.5f))
                   .AppendInterval(1f)
                   .Append(CanvasGroup.DOFade(0f, 0.2f))
                   .AppendCallback(Reset)
                   .Play();
        }
    }
}
