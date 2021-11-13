using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Sources.Data.Gameplay.Items;
using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class ChestData : GameplayData {
        public string Name;

        public int Width = 3;
        public int Length = 1;

        public ChanceData<int>[] RowWidthChances;
        public List<CardTypeChanceData> CardTypesChances;
        public ChanceData<EnemyData>[] EnemiesChances;
        public ChanceData<ItemData>[] ItemChances;
        public ChanceData<ChestData>[] ChestChances;

        [ShowAssetPreview]
        public Sprite Sprite;

        public LevelData GetLevel() {
            LevelData levelData = CreateInstance<LevelData>();
            levelData.Name = Name;
            levelData.Width = Width;
            levelData.Length = Length;
            levelData.Sprite = Sprite;
            
            levelData.ItemChances = ItemChances;
            levelData.EnemiesChances = EnemiesChances;
            levelData.ChestChances = ChestChances;
            levelData.CardTypesChances = CardTypesChances;
            levelData.RowWidthChances = RowWidthChances;
            return levelData;
        }
    }
}
