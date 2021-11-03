using System;
using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class Chest : ScriptableObject {
        public string Name;

        public int Width = 3;
        public int Length = 1;

        public Item[] Items;

        public Sprite Sprite;

        public Level GetLevel() {
            Level level = CreateInstance<Level>();
            level.Name = Name;
            level.Width = Width;
            level.Length = Length;
            level.Sprite = Sprite;
            level.Items = Items;
            level.Chests = Array.Empty<Chest>();
            level.Enemies = Array.Empty<Enemy>();
            level.Exits = Array.Empty<Level>();
            return level;
        }
    }
}
