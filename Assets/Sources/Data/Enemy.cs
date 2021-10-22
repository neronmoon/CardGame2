using UnityEngine;

namespace Sources.Data {
    [CreateAssetMenu]
    public class Enemy : ScriptableObject {
        public string Name;
        public int Health;
    }
}
