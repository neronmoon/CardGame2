using System.Collections.Generic;
using DG.Tweening;
using Leopotam.Ecs;
#if UNITY_EDITOR
using Leopotam.Ecs.UnityIntegration;
#endif
using Sources.Data;
using Sources.ECS.Animations;
using Sources.ECS.BaseInteractions;
using Sources.ECS.Components.Events;
using Sources.ECS.WorldInitialization;
using Sources.Unity.Support;
using UnityEngine;

namespace Sources {
    public class EcsStartup : MonoBehaviour {
        private EcsWorld world;

        private EcsSystems initSystems;
        private EcsSystems updateSystems;
        private EcsSystems fixedSystems;

        [SerializeField] private Configuration Configuration;
        [SerializeField] private SceneData SceneData;
        [SerializeField] private ObjectPool ObjectPool;

        private void Start() {
            DOTween.Init().SetCapacity(1000, 1000);

            world = new EcsWorld();
#if UNITY_EDITOR
            EcsWorldObserver.Create(world);
#endif
            initSystems = new EcsSystems(world);
            updateSystems = new EcsSystems(world);
            fixedSystems = new EcsSystems(world);

            // ONLY TO CALLED ONCE!!
            initSystems
                .Add(new InitGarbageEntity())
                ;

            updateSystems
                .Add(new InputSystem(), "input")
                .Add(new DragndropSystem())
                .Add(new LevelStartSystem()) // should be on top of non-technical systems
                .Add(new GenerateLevelLayoutSystem())
                .Add(new CreateLevelEntitiesSystem())
                .Add(new SpawnCardsGameObjectsSystem())
                .Add(new CardAnimationSystem())
                .OneFrame<StartLevelEvent>()
                ;
            // fixedSystems
            //     .Add(new InputSystem())
            // ;

            RuntimeData runtimeData = new RuntimeData();
            Camera camera = Camera.main;
            foreach (EcsSystems sys in new List<EcsSystems> { initSystems, updateSystems, fixedSystems }) {
                sys.Inject(Configuration)
                   .Inject(SceneData)
                   .Inject(runtimeData)
                   .Inject(ObjectPool)
                   .Inject(camera)
                   .Inject(updateSystems)
                   .Init();
            }
#if UNITY_EDITOR
            EcsSystemsObserver.Create(updateSystems);
#endif
            Debug.Log("[ECS] Systems initialized");
        }

        private void Update() {
            updateSystems?.Run();
        }

        private void FixedUpdate() {
            fixedSystems?.Run();
        }

        private void OnDestroy() {
            initSystems?.Destroy();
            updateSystems?.Destroy();
            fixedSystems?.Destroy();
            world.Destroy();
        }
    }
}
