using System.Collections.Generic;
using DG.Tweening;
using Leopotam.Ecs;
#if UNITY_EDITOR
using Leopotam.Ecs.UnityIntegration;
#endif
using Sources.Data;
using Sources.ECS;
using Sources.ECS.Animations;
using Sources.ECS.Audio;
using Sources.ECS.BaseInteractions;
using Sources.ECS.Components.Events;
using Sources.ECS.Movement;
using Sources.ECS.Visualization;
using Sources.ECS.WorldInitialization;
using Sources.ECS.GameplayActions;
using Sources.LevelGeneration;
using UnityEngine;

namespace Sources {
    public class EcsStartup : MonoBehaviour {
        private EcsWorld world;

        private EcsSystems initSystems;
        private EcsSystems updateSystems;
        private EcsSystems fixedSystems;

        [SerializeField] public Configuration Configuration;
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

                // should be on top of non-technical systems
                .Add(new LevelChangeSystem())

                // Cleanup
                .Add(new RecycleDiscardedEntitiesSystem())
                .Add(new DiscardLeftoverCardsSystem())

                // Level initialization
                .Add(new PopulateLevelWithEntitiesSystem())

                // Visualization
                .Add(new AnnounceLevelSystem())
                .Add(new SpawnCardsGameObjectsSystem())
                .Add(new DisplayCardStatsSystem())
                .Add(new AdjustBottomBarValuesSystem())
                .Add(new ShowCardDetailsSystem())

                // Player movement
                .Add(new DefinePlayerAvailableMovesSystem())
                .Add(new DisableDeadPlayerSystem())
                .Add(new PlayerMoveSystem())
                
                .Add(new DrawLevelMapSystem())

                // Gameplay actions
                .Add(new ActionsQueueSystem()) // for now i test new actions system
                // here goes all GameplayAction systems
                .Add(new ApplyDamageSystem())
                .Add(new HealSystem())
                .Add(new EnemyDropLootSystem())
                .Add(new DiscardCardsInPlayerRowSystem())
                .Add(new UpdateLevelLayoutOnPlayerMoveSystem())

                // Animations
                .Add(new CardAnimationSystem())
                .Add(new CleanupAnimatedSystem())
                // Sounds
                .Add(new AudioSystem())

                // Events
                .OneFrame<StartLevelEvent>()
                .OneFrame<DroppedEvent>()
                .OneFrame<DoubleClickedEvent>()
                .OneFrame<PlayerMovedEvent>()
                ;
            // fixedSystems
            //     .Add(new InputSystem())
            // ;

            RuntimeData runtimeData = new();
            LevelGenerator levelGenerator = new();
            CardEntityGenerator cardEntityGenerator = new(world, runtimeData, levelGenerator);
            Camera camera = Camera.main;
            foreach (EcsSystems sys in new List<EcsSystems> { initSystems, updateSystems, fixedSystems }) {
                sys.Inject(Configuration)
                   .Inject(SceneData)
                   .Inject(runtimeData)
                   .Inject(camera)
                   .Inject(levelGenerator)
                   .Inject(cardEntityGenerator)
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
