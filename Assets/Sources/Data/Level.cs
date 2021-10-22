using UnityEngine;

namespace Sources.Data {
    [CreateAssetMenu]
    public class Level : ScriptableObject {
        public string Name;
        public Enemy[] Enemies;
        public Level[] Exits;

        public int Length;
    }
}
