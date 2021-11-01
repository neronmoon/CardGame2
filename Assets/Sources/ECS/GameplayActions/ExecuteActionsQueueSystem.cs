using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.ECS.Animations.Components;
using Sources.ECS.Extensions;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;

namespace Sources.ECS.GameplayActions {
    public class ExecuteActionsQueueSystem : IEcsRunSystem {
        /// <summary>
        /// This system executes actions queue. It's adds component to entity and waits its' to dissapear
        /// After, it's adds next component from queue, and wait 
        /// </summary>
        private EcsWorld world;

        private EcsFilter<ActionsQueue> queueFilter;
        private EcsFilter<Animated> animated;

        private float lastRunTime;
        private const float Delay = 0.5f;

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

                bool animationIsBlocking = false;
                foreach (int i in animated) {
                    if (animated.Get1(i).Blocking) {
                        animationIsBlocking = true;
                        break;
                    }
                }

                if (queue.Count <= 0 || animationIsBlocking || !(Time.time - lastRunTime >= Delay)) {
                    continue;
                }

                lastRunTime = Time.time;
                IGameplayTrigger trigger = queue.Dequeue();
                // Add triggers dynamicaly
                switch (trigger) {
                    case Hit hit:
                        entity.Replace(hit);
                        actions.Enqueue(hit);
                        break;
                    case LevelChangeTrigger change:
                        entity.Replace(change);
                        actions.Enqueue(change);
                        break;
                    case CompleteStep step:
                        entity.Replace(step);
                        actions.Enqueue(step);
                        break;
                    default:
                        Debug.LogWarning("Unregistered trigger type");
                        break;
                }

                entity.Replace(new ActionsQueue { Queue = queue, ActiveActions = actions });
            }
        }
    }
}
