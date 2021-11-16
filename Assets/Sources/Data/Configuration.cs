using UnityEngine;

namespace Sources.Data {
    [CreateAssetMenu]
    public class Configuration : ScriptableObject {
        public GameObject CardPrefab;

        public AudioClip[] HitClips;
        public AudioClip[] CardSpawnClips;
        public AudioClip DeadClip;
        public AudioClip PotionClip;
    }
}
