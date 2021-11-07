using NaughtyAttributes;
using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class EnemyData : GameplayData {
        public int Health;
        
        [ShowAssetPreview]
        public Sprite Sprite;
    }
}
