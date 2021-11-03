using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.ECS.Components.Events;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;

namespace Sources.ECS.WorldInitialization {
    public class LevelStartSystem : IEcsInitSystem, IEcsRunSystem {
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

        private float time;

        public void Init() {
            time = Time.time;
        }

        public void Run() {
            // Add a bit delay, because animations are freezing at start
            // TODO: Fix this!
            if (runtimeData.CurrentLevel == null && Time.time - time > 1) {
                Level level = configuration.StartLevel;
                SetLevelState(level, levelGenerator.Generate(level, configuration.Character));
                runtimeData.GarbageEntity.Replace(new StartLevelEvent { Level = level });
            }

            // Change level logic: remember current level, Set new level state, throw event about level start
            foreach (int idx in change) {
                runtimeData.PlayerPath.Push(new KeyValuePair<Level, object[][]>(runtimeData.CurrentLevel, runtimeData.LevelLayout));
                LevelChange levelChange = change.Get1(idx);
                SetLevelState(levelChange.Level, levelChange.Layout);
                runtimeData.GarbageEntity.Replace(new StartLevelEvent { Level = levelChange.Level });
            }
        }

        private void SetLevelState(Level level, object[][] layout) {
            runtimeData.LevelLayout = layout;
            if (runtimeData.CurrentLevel != level) {
                runtimeData.CurrentLevel = level;
            }
        }
    }
}
