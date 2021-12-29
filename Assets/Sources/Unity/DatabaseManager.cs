using NaughtyAttributes;
using Sources.Database;
using UnityEngine;

namespace Sources.Unity {
    public class DatabaseManager : MonoBehaviour {
#if UNITY_EDITOR
        [Button("Recreate database")]
        public void SeedDatabase() {
            new ProtoDatabaseSeeder().Seed();
        }
#endif
    }
}
