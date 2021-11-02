using UnityEngine;
using Sources.Data.Gameplay;

namespace Sources.Data {
    [CreateAssetMenu]
    public class Configuration : ScriptableObject {
        public GameObject CardPrefab;
        public Character Character; // we can select character in menu in future
        public Level StartLevel;

        public AudioClip[] HitClips;
        public AudioClip[] CardSpawnClips;
        public AudioClip DeadClip;
        public AudioClip PotionClip;
    }
}
