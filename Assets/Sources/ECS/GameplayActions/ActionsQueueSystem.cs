using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay.Items;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;
using Sources.ECS.Extensions;
using UnityEngine;
using HealthPotion = Sources.ECS.Components.Gameplay.HealthPotion;

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
        private RuntimeData runtimeData;

        private float lastRunTime;
        private const float Delay = 0.5f;

        private Dictionary<Func<EcsEntity, bool>, Func<EcsEntity, object[]>> actions = new();
        private Dictionary<Func<EcsEntity, EcsEntity, bool>, Func<EcsEntity, EcsEntity, object[]>> moveActions = new();

        public ActionsQueueSystem() {
            DefineMoveAction(
                (entity, target) => target.Has<Enemy>() && target.Has<Health>(), // if this is true
                (entity, target) => new Hit { Source = target, Amount = target.Get<Health>().Amount } // then enqueue this component
            );
            DefineMoveAction(
                (entity, target) => target.Has<LevelExit>(),
                (entity, target) => {
                    LevelExit exit = target.Get<LevelExit>();
                    return new LevelChange { LevelData = exit.Data, Layout = exit.Layout };
                });
            DefineMoveAction(
                (entity, target) => target.Has<EquippableItem>() && entity.Has<Inventory>(),
                (entity, target) => entity.Get<Inventory>().Add(target.Get<EquippableItem>().Data)
            );
            DefineMoveAction(
                (entity, target) => target.Has<ConsumableItem>() && target.Has<HealthPotion>(),
                (entity, target) => new Heal { Amount = target.Get<HealthPotion>().Amount }
            );
            DefineAction(
                entity => entity.Has<Health>() && entity.Get<Health>().Amount <= 0 && entity.Get<Inventory>().Has<ResurrectStoneData>(),
                entity => {
                    Inventory inventory = entity.Get<Inventory>();
                    ResurrectStoneData stoneData = inventory.TakeOne<ResurrectStoneData>();
                    return new object[] {
                        inventory,
                        new Health { Amount = stoneData.HealthAfterResurrection }
                    };
                }
            );
            DefineAction(
                entity => entity.Has<Health>() && entity.Get<Health>().Amount <= 0 && !entity.Has<Dead>(),
                entity => new Dead()
            );
        }
        
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
                Queue<object> actions = actionsQueue.ActiveActions;
            
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

        private void DefineMoveAction(Func<EcsEntity, EcsEntity, bool> check, Func<EcsEntity, EcsEntity, object> component) {
            moveActions.Add(check, (e, t) => new[] { component.Invoke(e, t) });
        }

        private void DefineAction(Func<EcsEntity, bool> check, Func<EcsEntity, object> component) {
            actions.Add(check, (e) => new[] { component.Invoke(e) });
        }

        private void DefineMoveAction(Func<EcsEntity, EcsEntity, bool> check, Func<EcsEntity, EcsEntity, object[]> component) {
            moveActions.Add(check, component);
        }

        private void DefineAction(Func<EcsEntity, bool> check, Func<EcsEntity, object[]> component) {
            actions.Add(check, component);
        }

        private void PlanActions() {
            foreach (int idx in player) {
                EcsEntity entity = player.GetEntity(idx);
                ActionsQueue actionsQueue = GetQueue(player.GetEntity(idx));
                int countBefore = actionsQueue.Queue.Count;

                // Means player not moved
                if (entity.Has<PlayerMovedEvent>()) {
                    EcsEntity target = entity.Get<PlayerMovedEvent>().Target;
                    foreach ((Func<EcsEntity, EcsEntity, bool> key, Func<EcsEntity, EcsEntity, object[]> value) in moveActions) {
                        if (key.Invoke(entity, target)) {
                            foreach (object component in value.Invoke(entity, target)) {
                                actionsQueue.Queue.Enqueue(component);
                            }
                        }
                    }

                    if (countBefore == actionsQueue.Queue.Count) {
                        Debug.LogWarning("Player moved, but no actions planned!");
                    }
                }

                foreach ((Func<EcsEntity, bool> key, Func<EcsEntity, object[]> value) in actions) {
                    if (key.Invoke(entity)) {
                        foreach (object component in value.Invoke(entity)) {
                            actionsQueue.Queue.Enqueue(component);
                        }
                    }
                }

                if (countBefore != actionsQueue.Queue.Count || entity.Has<PlayerMovedEvent>()) {
                    // Finish move
                    actionsQueue.Queue.Enqueue(new CompleteStep());
                    entity.Replace(actionsQueue);
                }
            }
        }

        private void ExecuteActions() {
            foreach (int idx in queueFilter) {
                EcsEntity entity = queueFilter.GetEntity(idx);
                ActionsQueue actionsQueue = queueFilter.Get1(idx);

                if (actionsQueue.Queue.Count <= 0) {
                    continue;
                }

                lastRunTime = Time.time;
                object trigger = actionsQueue.Queue.Dequeue();

                entity.Replace(trigger);
                if (trigger is IShouldDisappear) {
                    actionsQueue.ActiveActions.Enqueue(trigger);
                }

                entity.Replace(new ActionsQueue {
                    Queue = actionsQueue.Queue,
                    ActiveActions = actionsQueue.ActiveActions
                });
            }
        }


        private static ActionsQueue GetQueue(EcsEntity entity) {
            if (!entity.Has<ActionsQueue>()) {
                entity.Replace(new ActionsQueue {
                    Queue = new Queue<object>(),
                    ActiveActions = new Queue<object>()
                });
            }

            return entity.Get<ActionsQueue>();
        }
    }
}
