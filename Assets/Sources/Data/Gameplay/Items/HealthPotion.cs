using UnityEngine;

namespace Sources.Data.Gameplay.Items {
    [CreateAssetMenu]
    public class HealthPotion : Item, IConsumableItemType {
        public int Amount;
    }
}
