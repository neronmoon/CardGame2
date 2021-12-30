using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.Unity.UI {
    public class DetailsLineView : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI Text;
        [SerializeField] private Image Image;

        public void Fill(Sprite image, string text) {
            Image.enabled = image != null;
            Image.sprite = image;

            Text.text = text;
        }
    }
}
