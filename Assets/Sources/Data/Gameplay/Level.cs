using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class Level : ScriptableObject {
        public string Name;
        public Enemy[] Enemies;
        public Level[] Exits;

        public int Width = 3;
        public int Length;
    }
}