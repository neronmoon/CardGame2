using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class Enemy : ScriptableObject {
        public int Health;
        public Sprite Sprite;
    }
}
