using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.Database.DataObject;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Processes;
using Sources.ECS.GameplayActions.Components;
using Sources.LevelGeneration;
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
            if (runtimeData.CurrentLevel == null && Time.time - time > 1) {
                Level levelData = Level.Get(1);
                Character character = Character.Get(1);
                runtimeData.CurrentCharacter = character;
                SetLevelState(levelData, levelGenerator.Generate(levelData, character));
                runtimeData.GarbageEntity.Replace(new StartLevelEvent { LevelData = levelData });
            }

            // Change level logic: remember current level, Set new level state, throw event about level start
            foreach (int idx in change) {
                runtimeData.PlayerPath.Push(new KeyValuePair<ILevelDefinition, object[][]>(runtimeData.CurrentLevel, runtimeData.LevelLayout));
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

        private void SetLevelState(ILevelDefinition level, object[][] layout) {
            runtimeData.LevelLayout = layout;
            if (runtimeData.CurrentLevel != level) {
                runtimeData.CurrentLevel = level;
            }
        }
    }
}
