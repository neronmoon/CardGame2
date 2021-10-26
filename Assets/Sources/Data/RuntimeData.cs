using Leopotam.Ecs;
using Sources.Data.Gameplay;
using Sources.ECS.Components;
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
        public EcsEntity GarbageEntity;
        public LevelPosition PlayerPosition = new LevelPosition { X = 0, Y = 0 };
        public object[][] LevelLayout;
    }
}
