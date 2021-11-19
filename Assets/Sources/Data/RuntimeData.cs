using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Database.DataObject;
using UnityEngine;

namespace Sources.Data {
    public struct Input {
        public Vector2 Position;
        public bool Primary;
        public bool Secondary;
    }

    public class RuntimeData {
        public Input Input;
        public Character CurrentCharacter;
        public ILevelDefinition CurrentLevel;
        public EcsEntity GarbageEntity;
        public object[][] LevelLayout;
        public bool PlayerIsDead = false;

        public Stack<KeyValuePair<ILevelDefinition, object[][]>> PlayerPath = new();
    }
}
