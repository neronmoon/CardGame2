using TMPro;
using UnityEngine;

namespace Sources.Unity {
    public class BottomBarView : MonoBehaviour {
        public TextMeshProUGUI HealthCounter;
        public TextMeshProUGUI GoldCounter;


        public void SetHealthCounterValues(int current, int max) {
            HealthCounter.text = $"{current}/{max}";
        }

        public void SetGoldCounterValues(int value) {
            GoldCounter.text = $"{value} coins";
        }
    }
}
