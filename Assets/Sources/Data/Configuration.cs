using UnityEngine;

namespace Sources.Data {
    [CreateAssetMenu]
    public class Configuration : ScriptableObject {
        public GameObject CardPrefab;
        public Character Character; // we can select character in menu in future
        public Level StartLevel;
    }
}
