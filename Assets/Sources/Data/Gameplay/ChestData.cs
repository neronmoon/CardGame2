using System;
using NaughtyAttributes;
using Sources.Data.Gameplay.Items;
using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class ChestData : GameplayData {
        public string Name;

        public int Width = 3;
        public int Length = 1;

        public ItemData[] Items;
        public ChanceData<ItemData>[] ItemChances;

        [ShowAssetPreview]
        public Sprite Sprite;

        public LevelData GetLevel() {
            LevelData levelData = CreateInstance<LevelData>();
            levelData.Name = Name;
            levelData.Width = Width;
            levelData.Length = Length;
            levelData.Sprite = Sprite;
            levelData.Items = Items;
            levelData.ItemChances = ItemChances;
            levelData.Chests = Array.Empty<ChestData>();
            levelData.Enemies = Array.Empty<EnemyData>();
            levelData.Exits = Array.Empty<LevelData>();
            return levelData;
        }
    }
}
