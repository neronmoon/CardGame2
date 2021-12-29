using Sources.Unity;
using UnityEngine;

namespace Sources.Data {
    public class SceneData : MonoBehaviour {
        public Transform OriginPoint;
        public Transform SpawnPoint;
        public Transform DiscardPoint;
        public Vector2 CardSpacing;

        public Transform WorldParent;

        public DeathScreenView DeathScreenView;
        public LevelAnnounceView LevelAnnounceView;
        public BottomBarView BottomBarView;
        public CardDetailsView CardDetailsView;
        public Transform LevelMapContainer;

        public AudioSource SFXAudioSource;
    }
}
