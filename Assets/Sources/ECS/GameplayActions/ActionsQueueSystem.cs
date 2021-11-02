using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;
using Sources.ECS.Extensions;
using UnityEngine;

namespace Sources.ECS.GameplayActions {
    public class ActionsQueueSystem : IEcsRunSystem {
        /// <summary>
        /// This system reacts to moved event and creates queue of gameplay actions
        /// So it's defines order of gameplay actions after dropped
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Player> player;
        private EcsFilter<PlayableCard, Spawned> cards;

        private EcsFilter<ActionsQueue> queueFilter;
        private EcsFilter<Animated> animated;

        private float lastRunTime;
        private const float Delay = 0.5f;

        public void Run() {
            bool animationIsBlocking = false;
            foreach (int i in animated) {
                if (animated.Get1(i).Blocking) {
                    animationIsBlocking = true;
                    break;
                }
            }

            foreach (int idx in queueFilter) {
                EcsEntity entity = queueFilter.GetEntity(idx);
                ActionsQueue actionsQueue = queueFilter.Get1(idx);
                Queue<IGameplayTrigger> actions = actionsQueue.ActiveActions;

                // Remove previous frame components
                while (actions.Count > 0) {
                    entity.Del(actions.Dequeue().GetType());
                }
            }

            if (animationIsBlocking || !(Time.time - lastRunTime >= Delay)) {
                return;
            }

            ExecuteActions();
            PlanActions();
        }

        private void ExecuteActions() {
            foreach (int idx in queueFilter) {
                EcsEntity entity = queueFilter.GetEntity(idx);
                ActionsQueue actionsQueue = queueFilter.Get1(idx);
                Queue<IGameplayTrigger> queue = actionsQueue.Queue;
                Queue<IGameplayTrigger> actions = actionsQueue.ActiveActions;

                if (queue.Count <= 0) {
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
                    case Heal heal:
                        entity.Replace(heal);
                        actions.Enqueue(heal);
                        break;
                    case LevelChangeTrigger change:
                        entity.Replace(change);
                        actions.Enqueue(change);
                        break;
                    case Dead dead:
                        entity.Replace(dead);
                        // actions.Enqueue(change);
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

        private void PlanActions() {
            foreach (int idx in player) {
                EcsEntity entity = player.GetEntity(idx);

                bool added = false;
                ActionsQueue actionsQueue = GetQueue(player.GetEntity(idx));

                // Means player not moved
                if (entity.Has<PlayerMovedEvent>()) {
                    EcsEntity target = entity.Get<PlayerMovedEvent>().Target;
                    if (target.Has<Enemy>() && target.Has<Health>()) {
                        actionsQueue.Queue.Enqueue(new Hit { Source = target, Amount = target.Get<Health>().Amount });
                        added = true;
                    } else if (target.Has<LevelExit>()) {
                        actionsQueue.Queue.Enqueue(new LevelChangeTrigger { Level = target.Get<LevelExit>().Data });
                        added = true;
                    } else if (target.Has<HealthPotion>()) {
                        actionsQueue.Queue.Enqueue(new Heal { Amount = target.Get<HealthPotion>().Amount });
                        added = true;
                    } else {
                        Debug.LogWarning("Player moved, but no actions planned!");
                    }
                }

                if (entity.Has<Health>() && entity.Get<Health>().Amount <= 0 && !entity.Has<Dead>()) {
                    actionsQueue.Queue.Enqueue(new Dead());
                    added = true;
                }

                if (added) {
                    // Finish move
                    actionsQueue.Queue.Enqueue(new CompleteStep());
                    entity.Replace(actionsQueue);
                }
            }
        }

        private static ActionsQueue GetQueue(EcsEntity entity) {
            if (!entity.Has<ActionsQueue>()) {
                entity.Replace(new ActionsQueue {
                    Queue = new Queue<IGameplayTrigger>(),
                    ActiveActions = new Queue<IGameplayTrigger>()
                });
            }

            return entity.Get<ActionsQueue>();
        }
    }
}
