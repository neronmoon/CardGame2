using Leopotam.Ecs;
using Sources.Data.Gameplay;
using UnityEngine;

namespace Sources.Data {
    public struct Input {
        public Vector2 Position;
        public bool Primary;
        public bool Secondary;
    }

    public class RuntimeData {
        public Input Input;
        public Level CurrentLevel;
        public int CurrentPlayerPosition;
        public EcsEntity GarbageEntity;
        public object[][] LevelLayout;
    }
}
