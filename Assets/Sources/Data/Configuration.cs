using UnityEngine;

namespace Sources.Data {
    [CreateAssetMenu]
    public class Configuration : ScriptableObject {
        public GameObject PlayerPrefab;
        public GameObject ItemCardPrefab;
        public GameObject LevelCardPrefab;
        public GameObject CardPrefab;

        public AudioClip[] HitClips;
        public AudioClip CardSpawnClip;
        public AudioClip DeadClip;
        public AudioClip PotionClip;
    }
}
