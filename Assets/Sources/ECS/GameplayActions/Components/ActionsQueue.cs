using System.Collections.Generic;

namespace Sources.ECS.GameplayActions.Components {
    public struct ActionsQueue {
        public Queue<object> Queue;
        public Queue<object> ActiveActions;
    }
}
