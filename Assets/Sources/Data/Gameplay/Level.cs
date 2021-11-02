using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class Level : ScriptableObject {
        public string Name;
        public Enemy[] Enemies;
        public Item[] Items;
        public Level[] Exits;

        public int Width = 3;
        public int Length;

        public Sprite Sprite;
    }
}
