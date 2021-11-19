using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sources.ECS.Animations.Components {
    public struct CardAnimationState {
        public Vector3 InitScale;
        public List<Type> Components;
        public bool SpawnAnimationStarted; // used for playing audio clip
        public bool SpawnAnimationCompleted; // Used for calculation of spawn delay
    }
}
