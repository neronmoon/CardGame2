using UnityEngine;

namespace Sources.Data.Gameplay.Items {
    [CreateAssetMenu]
    public class HealthPotionData : ItemData, IConsumableItemType {
        public int Amount;
    }
}
