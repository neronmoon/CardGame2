using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Sources.Unity {
    public class CardView : MonoBehaviour {
        public SpriteRenderer Sprite;

        public TextMeshProUGUI HealthText;
        public GameObject[] HealthObjects;

        public TextMeshProUGUI NameText;
        public GameObject[] NameObjects;
        public int AdditionalSortOrder = 0;

        private Dictionary<Component, int> sortOrders = new Dictionary<Component, int>(10);

        private void Start() {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
                sortOrders.Add(renderer, renderer.sortingOrder);
            }
            foreach (Canvas canvas in GetComponentsInChildren<Canvas>()) {
                sortOrders.Add(canvas, canvas.sortingOrder);
            }
        }

        private void Update() {
            foreach (KeyValuePair<Component, int> x in sortOrders) {
                int order = (int)(x.Value + transform.position.y) + AdditionalSortOrder;
                switch (x.Key) {
                    case SpriteRenderer renderer:
                        renderer.sortingOrder = order;
                    break;
                    case Canvas renderer:
                        renderer.sortingOrder = order;
                        break;
                }
            }
        }
    }
}
