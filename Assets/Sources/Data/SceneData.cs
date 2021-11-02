using Sources.Unity;
using UnityEngine;

namespace Sources.Data {
    public class SceneData : MonoBehaviour {
        public Transform OriginPoint;
        public Transform SpawnPoint;
        public Transform DiscardPoint;
        public Vector2 CardSpacing;

        public DeathScreenView DeathScreenView;
        public LevelAnnounceView LevelAnnounceView;

        public AudioSource SFXAudioSource;
    }
}
