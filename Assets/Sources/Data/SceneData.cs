using Sources.Unity;
using UnityEngine;

namespace Sources.Data {
    public class SceneData : MonoBehaviour {
        public Transform OriginPoint;
        public Transform SpawnPoint;
        public Transform DiscardPoint;
        public Vector2 CardSpacing;

        public DeathScreenView DeathScreen;

        public AudioSource SFXAudioSource;
    }
}
