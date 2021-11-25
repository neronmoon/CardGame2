using System;
using System.Collections.Generic;
using System.Linq;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Database.DataObject;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;
using Sources.ECS.Extensions;
using UnityEngine;
using Enemy = Sources.ECS.Components.Gameplay.Enemy;
using HealthPotion = Sources.ECS.Components.Gameplay.HealthPotion;

namespace Sources.ECS.GameplayActions {
    public class ActionsQueueSystem : IEcsRunSystem {
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

        public ActionsQueueSystem() {
            // Place actions in right order!!
            // Further action can relate on entity state from prev action.
            // So After HitAction we can check if HP <=0 and use stone to add HP before player is dead

            // Move actions are executed in one frame (PlayerMovedEvent)
            DefineMoveAction(
                (entity, target) => target.Has<Enemy>() && target.Has<Health>(), // if this is true
                (entity, target) => new Hit { Source = target, Amount = target.Get<Health>().Value } // then add this component(s)
            );
            DefineMoveAction(
                (entity, target) => target.Has<LevelEntrance>(),
                (entity, target) => {
                    LevelEntrance entrance = target.Get<LevelEntrance>();
                    return new LevelChange { LevelData = entrance.Data, Layout = entrance.Layout };
                });
            DefineMoveAction(
                (entity, target) => target.Has<EquippableItem>() && entity.Has<Inventory>(),
                (entity, target) => entity.Get<Inventory>().Add(target.Get<EquippableItem>().Data)
            );
            DefineMoveAction(
                (entity, target) => target.Has<ConsumableItem>() && target.Has<CardEffects>(),
                (entity, target) => {
                    List<ItemEffect> effects = target.Get<CardEffects>().Effects;
                    List<object> components = new(effects.Count);
                    foreach (ItemEffect effect in effects) {
                        switch (effect.Name) {
                            case "Heal":
                                components.Add(new Heal { Amount = (int)effect.Value });
                                break;
                        }
                    }

                    return components.ToArray();
                }
            );

            // Resurrection before death!
            // DefineAction(
            //     entity => entity.Has<Health>() && entity.Get<Health>().Amount <= 0 && entity.Get<Inventory>().HasWithEffect("Resurrection"),
            //     entity => {
            //         Inventory inventory = entity.Get<Inventory>();
            //         Item item = inventory.TakeOneWithEffect("Resurrection");
            //         return new object[] {
            //             inventory,
            //             new Health { Amount = (int)item.Effects.First(e => e.Name == "Resurrection").Value }
            //         };
            //     }
            // );

            // This is final death!
            DefineAction(
                entity => entity.Has<Health>() && entity.Get<Health>().Value <= 0 && !entity.Has<Dead>(),
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
