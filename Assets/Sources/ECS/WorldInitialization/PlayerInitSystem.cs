using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.Unity.Support;
using UnityEngine;

namespace Sources.ECS.WorldInitialization {
    public class PlayerInitSystem : IEcsInitSystem {
        /// <summary>
        /// Creating player
        /// </summary>
        private EcsWorld world;

        private Configuration configuration;
        private ObjectPool pool;

        public void Init() {
            EcsEntity playerEntity = world.NewEntity();

            playerEntity.Replace(new Player());
            GameObject playerObj = pool.Spawn(configuration.CardPrefab);
            playerObj.tag = "Entity";
            playerObj.name = "Player";
            playerEntity.Replace(new VisualObject { Object = playerObj });
        }
    }
}
