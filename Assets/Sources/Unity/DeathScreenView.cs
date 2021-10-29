using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.Unity {
    public class DeathScreenView : MonoBehaviour {
        public Image Background;
        public CanvasGroup Content;

        private void Start() {
            Background.DOFade(0f, 0f);
            Content.DOFade(0f, 0f);
        }

        public void Show() {
            DOTween.Sequence()
                   .Append(Background.DOFade(1f, 0.3f))
                   .AppendInterval(0.5f)
                   .Append(Content.DOFade(1f, 0.4f))
                   .Play();
        }
    }
}
