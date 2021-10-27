using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.ECS.Animations.Components;
using Sources.ECS.Extensions;
using Sources.GameplayActions.Components;
using UnityEngine;

namespace Sources.GameplayActions {
    public class ExecuteActionsQueueSystem : IEcsRunSystem {
        /// <summary>
        /// This system executes actions queue. It's adds component to entity and waits its' to dissapear
        /// After, it's adds next component from queue, and wait 
        /// </summary>
        private EcsWorld world;

        private EcsFilter<ActionsQueue> queueFilter;

        public void Run() {
            foreach (int idx in queueFilter) {
                EcsEntity entity = queueFilter.GetEntity(idx);
                ActionsQueue actionsQueue = queueFilter.Get1(idx);
                Queue<IGameplayTrigger> queue = actionsQueue.Queue;
                Queue<IGameplayTrigger> actions = actionsQueue.ActiveActions;

                // Remove previous frame components
                while (actions.Count > 0) {
                    entity.Del(actions.Dequeue().GetType());
                }

                if (queue.Count > 0 && !entity.Has<Animated>()) {
                    IGameplayTrigger trigger = queue.Dequeue();
                    // Add triggers dynamicly
                    switch (trigger) {
                        case Hit hit:
                            entity.Replace(hit);
                            actions.Enqueue(hit);
                            Debug.Log("Executing hit");
                            break;
                        default:
                            Debug.Log("Unregistered trigger type");
                            break;
                    }

                    entity.Replace(new ActionsQueue { Queue = queue, ActiveActions = actions });
                }
            }
        }
    }
}
