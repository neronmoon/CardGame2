using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.Unity {
    public class LevelMapChunk : MonoBehaviour {
        public Image Icon;
        public Image Bar;

        public Sprite BossChunkIcon;
        public Sprite FightChunkIcon;
        public Sprite ItemChunkIcon;
        public Sprite ChestChunkIcon;
        public Sprite ExitChunkIcon;
        public Sprite CurrentChunkIcon;

        public Color BossIconColor;

        public Color DefaultIconColor;

        [HideInInspector]
        public int Y;

        public void SetYPosition(int y) {
            Y = y;
        }

        public void SetType(string type) {
            Icon.color = DefaultIconColor;
            Icon.enabled = !string.IsNullOrEmpty(type);

            switch (type) {
                case "boss":
                    Icon.sprite = BossChunkIcon;
                    Icon.color = BossIconColor;
                    break;
                case "fight":
                    Icon.sprite = FightChunkIcon;
                    break;
                case "item":
                    Icon.sprite = ItemChunkIcon;
                    break;
                case "chest":
                    Icon.sprite = ChestChunkIcon;
                    break;
                case "exit":
                    Icon.sprite = ExitChunkIcon;
                    break;
                case "player":
                    Icon.sprite = CurrentChunkIcon;
                    break;
            }
        }

        public void SetState(bool active) {
            float duration = 0.3f;
            Icon.DOFade(active ? 1f : 0f, duration);
            Bar.DOFade(active ? 1f : 0.5f, duration);
        }

        public void Destroy() {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
