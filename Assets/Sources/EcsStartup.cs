using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions;
using Sources.ECS.WorldInitialization;
using Sources.Unity.Support;
using UnityEngine;

namespace Sources {
    public class EcsStartup : MonoBehaviour {
        private EcsWorld world;

        private EcsSystems initSystems;
        private EcsSystems systems;
        private EcsSystems fixedSystems;

        [SerializeField] private Configuration Configuration;
        [SerializeField] private ObjectPool ObjectPool;

        private void Start() {
            world = new EcsWorld();

            initSystems = new EcsSystems(world);
            systems = new EcsSystems(world);
            fixedSystems = new EcsSystems(world);

            initSystems
                .Add(new PlayerInitSystem());
            systems
                .Add(new InputSystem());
            // fixedSystems
            //     .Add(new InputSystem());

            RuntimeData runtimeData = new RuntimeData();
            foreach (EcsSystems sys in new List<EcsSystems> { initSystems, systems, fixedSystems }) {
                sys.Inject(Configuration);
                sys.Inject(runtimeData);
                sys.Inject(ObjectPool);
                sys.Init();
            }

            Debug.Log("[ECS] Systems initialized");
        }

        private void Update() {
            systems?.Run();
        }

        private void FixedUpdate() {
            fixedSystems?.Run();
        }

        private void OnDestroy() {
            initSystems?.Destroy();
            systems?.Destroy();
            fixedSystems?.Destroy();
            world.Destroy();
        }
    }
}
