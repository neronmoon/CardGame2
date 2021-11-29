using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.Unity {
    public class LevelMapChunk : MonoBehaviour {
        public Image Icon;
        public Image Bar;

        public int Y;

        public void SetYPosition(int y) {
            Y = y;
        }
        
        public void SetState(bool active) {
            Icon.gameObject.SetActive(active);
            Bar.DOFade(active ? 1f : 0.5f, 0.3f);
        }
    }
}
