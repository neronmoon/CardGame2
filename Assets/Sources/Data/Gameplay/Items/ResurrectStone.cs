using UnityEngine;

namespace Sources.Data.Gameplay.Items {
    
    [CreateAssetMenu]
    public class ResurrectStone : Item, IEquippableItemType {
        public int HealthAfterResurrection = 1;
    }
}
