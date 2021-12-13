using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Sources.Unity.Support {
    public class View : MonoBehaviour {
        [Foldout("Show/Hide")]
        public Transform[] ShowHideIgnoreObjects;

        [Foldout("Show/Hide")]
        public float ShowHideTime = 0.3f;

        [Foldout("Show/Hide")]
        public bool InitialState = false;

        private void Awake() {
            float target = InitialState ? 1f : 0f;
            setFade(target, 0f);
        }

        [Button("Show")]
        public void Show() {
            setFade(1f, ShowHideTime);
        }

        [Button("Hide")]
        public void Hide() {
            setFade(0f, ShowHideTime);
        }

        private void setFade(float target, float time) {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
                if (!ShowHideIgnoreObjects.Contains(renderer.transform)) {
                    renderer.DOFade(target, time);
                }
            }

            foreach (CanvasGroup renderer in GetComponentsInChildren<CanvasGroup>()) {
                if (!ShowHideIgnoreObjects.Contains(renderer.transform)) {
                    renderer.DOFade(target, time).OnComplete(() => renderer.blocksRaycasts = target != 0f);
                }
            }
        }

        public void SetLayer(string layer) {
            foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>(true)) {
                trans.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }
    }
}
