using System.Collections.Generic;

namespace Sources.GameplayActions.Components {
    public struct ActionsQueue {
        public Queue<IGameplayTrigger> Queue;
        public Queue<IGameplayTrigger> ActiveActions;
    }
}
