using UnityEngine;

namespace Sources.Data.Gameplay.Items {
    
    [CreateAssetMenu]
    public class ResurrectStoneData : ItemData, IEquippableItemType {
        public int HealthAfterResurrection = 1;
    }
}
