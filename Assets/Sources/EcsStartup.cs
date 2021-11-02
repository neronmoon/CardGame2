using System.Collections.Generic;
using DG.Tweening;
using Leopotam.Ecs;
#if UNITY_EDITOR
using Leopotam.Ecs.UnityIntegration;
#endif
using Sources.Data;
using Sources.ECS.Animations;
using Sources.ECS.Audio;
using Sources.ECS.BaseInteractions;
using Sources.ECS.Components.Events;
using Sources.ECS.Movement;
using Sources.ECS.Visualization;
using Sources.ECS.WorldInitialization;
using Sources.ECS.GameplayActions;
using UnityEngine;

namespace Sources {
    public class EcsStartup : MonoBehaviour {
        private EcsWorld world;

        private EcsSystems initSystems;
        private EcsSystems updateSystems;
        private EcsSystems fixedSystems;

        [SerializeField] private Configuration Configuration;
        [SerializeField] private SceneData SceneData;

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
                .Add(new InputSystem())
                .Add(new DragndropSystem())
                .Add(new LevelStartSystem()) // should be on top of non-technical systems

                // Cleanup
                .Add(new RecycleDiscardedEntitiesSystem())
                .Add(new DiscardLeftoverCardsSystem())

                // Level initialization
                .Add(new GenerateLevelLayoutSystem())
                .Add(new PopulateLevelWithEntitiesSystem())
                .Add(new SetCurrentPlayerPositionSystem())

                // Visualization
                .Add(new AnnounceLevelSystem())
                .Add(new SpawnCardsGameObjectsSystem())
                .Add(new DisplayCardStatsSystem())

                // Player movement
                .Add(new DefinePlayerAvailableMovesSystem())
                .Add(new DisableDeadPlayerSystem())
                .Add(new PlayerMoveSystem())

                // Gameplay actions
                .Add(new ActionsQueueSystem())
                // here goes all GameplayAction systems
                .Add(new ApplyDamageSystem())
                .Add(new HealSystem())
                .Add(new DiscardCardsInPlayerRowSystem())

                // Animations
                .Add(new CardAnimationSystem())
                .Add(new CleanupAnimatedSystem())
                // Sounds
                .Add(new AudioSystem())

                // Events
                .OneFrame<StartLevelEvent>()
                .OneFrame<DroppedEvent>()
                .OneFrame<PlayerMovedEvent>()
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
