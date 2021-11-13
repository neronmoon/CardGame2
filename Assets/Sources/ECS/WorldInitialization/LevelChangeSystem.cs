using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Processes;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;

namespace Sources.ECS.WorldInitialization {
    public class LevelChangeSystem : IEcsInitSystem, IEcsRunSystem {
        /// <summary>
        /// Creates entity with one-frame component to indicate level start
        /// Also changes runtimeData's current level
        /// </summary>
        private EcsWorld world;

        private Configuration configuration;
        private RuntimeData runtimeData;
        private LevelGenerator levelGenerator;

        private EcsFilter<StartLevelEvent> start;
        private EcsFilter<LevelChange> change;
        
        private EcsFilter<Animated> animated;
        private EcsFilter<LevelIsChanging> isChanging;

        private float time;

        public void Init() {
            time = Time.time;
        }

        public void Run() {
            // Add a bit delay, because animations are freezing at start
            // TODO: Fix this!
            if (runtimeData.CurrentLevelData == null && Time.time - time > 1) {
                LevelData levelData = configuration.StartLevelData;
                SetLevelState(levelData, levelGenerator.Generate(levelData, configuration.CharacterData));
                runtimeData.GarbageEntity.Replace(new StartLevelEvent { LevelData = levelData });
            }

            // Change level logic: remember current level, Set new level state, throw event about level start
            foreach (int idx in change) {
                runtimeData.PlayerPath.Push(new KeyValuePair<LevelData, object[][]>(runtimeData.CurrentLevelData, runtimeData.LevelLayout));
                LevelChange levelChange = change.Get1(idx);
                SetLevelState(levelChange.LevelData, levelChange.Layout);
                runtimeData.GarbageEntity.Replace(new StartLevelEvent { LevelData = levelChange.LevelData });
                runtimeData.GarbageEntity.Replace(new LevelIsChanging());
            }
            
            foreach (int idx in isChanging) {
                if (animated.IsEmpty()) {
                    isChanging.GetEntity(idx).Del<LevelIsChanging>();
                }
            }
        }

        private void SetLevelState(LevelData levelData, object[][] layout) {
            runtimeData.LevelLayout = layout;
            if (runtimeData.CurrentLevelData != levelData) {
                runtimeData.CurrentLevelData = levelData;
            }
        }
    }
}
