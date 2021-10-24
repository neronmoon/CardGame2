using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class Enemy : ScriptableObject {
        public string Name;
        public int Health;
    }
}
