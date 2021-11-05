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
    public class ActionsQueueAlterSystem : IEcsRunSystem {
        /// <summary>
        /// This system adds components to player entity according to defined rules
        /// But it does it with animations checks and some timeout between component packs
        /// After all rules are played - system add CompleteStep component
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Player> player;
        private EcsFilter<PlayableCard, Spawned> cards;

        private EcsFilter<ActionsQueue> queueFilter;
        private EcsFilter<Animated> animated;
        private RuntimeData runtimeData;

        private float lastRunTime;
        private bool lastRunChanged = false;
        private const float Delay = 0.5f;

        private Dictionary<Func<EcsEntity, bool>, Func<EcsEntity, object[]>> actions = new();
        private Dictionary<Func<EcsEntity, EcsEntity, bool>, Func<EcsEntity, EcsEntity, object[]>> moveActions = new();

        public ActionsQueueAlterSystem() {
            // Place actions in right order!!
            // Further action can relate on entity state from prev action.
            // So After HitAction we can check if HP <=0 and use stone to add HP before player is dead
            
            // Move actions are executed in one frame (PlayerMovedEvent)
            DefineMoveAction(
                (entity, target) => target.Has<Enemy>() && target.Has<Health>(), // if this is true
                (entity, target) => new Hit { Source = target, Amount = target.Get<Health>().Amount } // then add this component(s)
            );
            DefineMoveAction(
                (entity, target) => target.Has<LevelExit>(),
                (entity, target) => {
                    LevelExit exit = target.Get<LevelExit>();
                    return new LevelChange { Level = exit.Data, Layout = exit.Layout };
                });
            DefineMoveAction(
                (entity, target) => target.Has<EquippableItem>() && entity.Has<Inventory>(),
                (entity, target) => entity.Get<Inventory>().Add(target.Get<EquippableItem>().Data)
            );
            DefineMoveAction(
                (entity, target) => target.Has<ConsumableItem>() && target.Has<HealthPotion>(),
                (entity, target) => new Heal { Amount = target.Get<HealthPotion>().Amount }
            );

            // Resurrection before death!
            DefineAction(
                entity => entity.Has<Health>() && entity.Get<Health>().Amount <= 0 && entity.Get<Inventory>().Has<ResurrectStone>(),
                entity => {
                    Inventory inventory = entity.Get<Inventory>();
                    ResurrectStone stone = inventory.TakeOne<ResurrectStone>();
                    return new object[] {
                        inventory,
                        new Health { Amount = stone.HealthAfterResurrection }
                    };
                }
            );

            // This is final death!
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

            // Cleanup last frame components (Heal, Hit, etc...)
            foreach (int idx in player) {
                EcsEntity entity = player.GetEntity(idx);
                Type[] types = { };
                entity.GetComponentTypes(ref types);
                foreach (Type type in types) {
                    if (typeof(IShouldDisappear).IsAssignableFrom(type)) {
                        entity.Del(type);
                    }
                }

                if (
                    entity.Has<PlayerMovedEvent>() || // If player moved -- we need to exec actions anyway
                    !animationIsBlocking && Time.time - lastRunTime >= Delay
                ) {
                    ExecuteActions();
                }
            }
        }

        private void ExecuteActions() {
            foreach (int idx in player) {
                EcsEntity entity = player.GetEntity(idx);

                bool changed = false;
                if (entity.Has<PlayerMovedEvent>()) {
                    foreach ((Func<EcsEntity, EcsEntity, bool> key, Func<EcsEntity, EcsEntity, object[]> value) in moveActions) {
                        EcsEntity target = entity.Get<PlayerMovedEvent>().Target;
                        if (key.Invoke(entity, target)) {
                            changed = true;
                            foreach (object component in value.Invoke(entity, target)) {
                                entity.Replace(component);
                            }
                        }
                    }

                    // Move actions should all be triggered at one frame (because PlayerMovedEvent lives only one frame) 
                    if (changed) {
                        lastRunChanged = true;
                        lastRunTime = Time.time;
                        return;
                    }

                    Debug.LogWarning("Player moved, but no actions executed!");
                }

                foreach ((Func<EcsEntity, bool> key, Func<EcsEntity, object[]> value) in actions) {
                    if (key.Invoke(entity)) {
                        changed = true;
                        foreach (object component in value.Invoke(entity)) {
                            entity.Replace(component);
                        }
                    }

                    // Simple actions not bound to PlayerMovedEvent, so we can apply them with timeout
                    if (changed) {
                        lastRunChanged = true;
                        lastRunTime = Time.time;
                        return;
                    }
                }

                if (lastRunChanged) {
                    lastRunChanged = false;
                    lastRunTime = Time.time;
                    entity.Replace(new CompleteStep());
                }
            }
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
    }
}
