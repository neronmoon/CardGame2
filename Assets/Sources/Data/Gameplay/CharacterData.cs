using NaughtyAttributes;
using UnityEngine;

namespace Sources.Data.Gameplay {
    [CreateAssetMenu]
    public class CharacterData : GameplayData {
        public int Health;

        [ShowAssetPreview]
        public Sprite Sprite;
    }
}
