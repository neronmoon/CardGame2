using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Sources.Data.Gameplay.Items;
using TypeReferences;
using UnityEngine;

namespace Sources.Data.Gameplay {
    [Serializable]
    public struct ChanceData<T>{
        public T Data;
        public int Chance;
    }

    [Serializable]
    public struct CardTypeChanceData {
        [ClassExtends(typeof(GameplayData))]
        public ClassTypeReference Data;

        public int Chance;
    }

    [CreateAssetMenu]
    public class LevelData : GameplayData {
        public string Name;
        public EnemyData[] Enemies;
        public ItemData[] Items;
        public ChestData[] Chests;
        public LevelData[] Exits;

        public ChanceData<int>[] RowWidthChances;
        public List<CardTypeChanceData> CardTypesChances;
        public ChanceData<EnemyData>[] EnemiesChances;
        public ChanceData<ItemData>[] ItemChances;
        public ChanceData<ChestData>[] ChestChances;

        public int Width = 3;
        public int Length;

        [ShowAssetPreview]
        public Sprite Sprite;
    }
}
