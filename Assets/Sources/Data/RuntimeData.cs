using System.Collections.Generic;
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
        public LevelPosition PlayerPosition = new() {X = 0, Y = 0};
        public object[][] LevelLayout;
        public bool PlayerIsDead = false;

        public Stack<KeyValuePair<Level, object[][]>> PlayerPath = new();
    }
}
