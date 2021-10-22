using System.Collections.Generic;
using Leopotam.Ecs;
#if UNITY_EDITOR
using Leopotam.Ecs.UnityIntegration;
#endif
using Sources.Data;
using Sources.ECS.BaseInteractions;
using Sources.ECS.Components.Events;
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
#if UNITY_EDITOR
            EcsWorldObserver.Create(world);
#endif
            initSystems = new EcsSystems(world);
            systems = new EcsSystems(world);
            fixedSystems = new EcsSystems(world);

            // ONLY TO CALLED ONCE!!
            initSystems
                .Add(new InitGarbageEntity())
                ;

            systems
                .Add(new InputSystem())
                .Add(new HoverSystem())
                .Add(new ClickSystem())
                .Add(new LevelStartSystem()) // should be on top of non-technical systems
                .Add(new GenerateLevelLayoutSystem())
                .Add(new CreateLevelEntitiesSystem())
                .Add(new SpawnCardsGameObjectsSystem())
                .OneFrame<StartLevelEvent>()
                ;
            // fixedSystems
            //     .Add(new InputSystem())
            // ;

            RuntimeData runtimeData = new RuntimeData();
            Camera camera = Camera.main;
            foreach (EcsSystems sys in new List<EcsSystems> { initSystems, systems, fixedSystems }) {
                sys.Inject(Configuration)
                   .Inject(runtimeData)
                   .Inject(ObjectPool)
                   .Inject(camera)
                   .Init();
            }
#if UNITY_EDITOR
            EcsSystemsObserver.Create(systems);
#endif
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
