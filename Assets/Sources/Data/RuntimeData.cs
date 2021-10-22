using UnityEngine;

namespace Sources.Data {
    public struct Input {
        public Vector2 Position;
        public bool Primary;
        public bool Secondary;
    }

    public class RuntimeData {
        public Input Input;
    }
}
