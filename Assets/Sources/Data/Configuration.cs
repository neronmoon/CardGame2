using UnityEngine;
using Sources.Data.Gameplay;

namespace Sources.Data {
    [CreateAssetMenu]
    public class Configuration : ScriptableObject {
        public GameObject CardPrefab;
        public CharacterData CharacterData; // we can select character in menu in future
        public LevelData StartLevelData;

        public AudioClip[] HitClips;
        public AudioClip[] CardSpawnClips;
        public AudioClip DeadClip;
        public AudioClip PotionClip;
    }
}
