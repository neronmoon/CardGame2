using System;
using System.Collections.Generic;
using DG.Tweening;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.Components.Processes;
using Sources.ECS.GameplayActions.Components;
using Sources.ECS.Extensions;
using Sources.ECS.GameplayActions.Actions;
using UnityEngine;

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
        private EcsFilter<Enemy, Health, LevelPosition, Spawned>.Exclude<Dead> enemiesFilter;

        private EcsFilter<Animated> animated;
        private RuntimeData runtimeData;

        private float lastRunTime;
        private bool lastRunChanged;
        private const float Delay = 0f;

        private IEnumerable<IGameplayMoveAction> GetMoveActions() {
            // Place actions in right order!!
            // Further action can relate on entity state from prev action.
            // So After HitAction we can check if HP <=0 and use stone to add HP before player is dead
            return new IGameplayMoveAction[] {
                new PlayerAttackAction(),
                new PickupItemAction(),
                new ConsumeItemAction(),
                new EnterLevelAction(),
            };
        }

        private IEnumerable<IGameplayAction> GetActions() {
            return new IGameplayAction[] {
                new GetAttackedByAggressiveEnemyAction(enemiesFilter, runtimeData),
                new PreHitAttackAction(),
                new ResurrectAction(),
                new DieAction()
            };
        }

        public void Run() {
            bool animationIsBlocking = DOTween.TotalPlayingTweens() != 0;

            // Cleanup last frame components (Heal, Hit, etc...)
            foreach (int idx in player) {
                EcsEntity entity = player.GetEntity(idx);
                Type[] types = { };
                entity.GetComponentTypes(ref types);
                foreach (Type type in types) {
                    if (typeof(IShouldDisappear).IsAssignableFrom(type)) {
                        entity.Del(type);
                    }

                    if (typeof(CompleteStep) == type) {
                        entity.Del<StepInProgress>();
                    }
                }

                if (
                    entity.Has<PlayerMovedEvent>() || // If player moved -- we need to exec actions anyway
                    !animationIsBlocking &&
                    Time.time - lastRunTime >= Delay
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
                    foreach (IGameplayMoveAction action in GetMoveActions()) {
                        EcsEntity target = entity.Get<PlayerMovedEvent>().Target;
                        if (action.ShouldAct(entity, target)) {
                            Debug.Log($"[Actions Queue] Executing {action.GetType().Name}");
                            changed = true;
                            foreach (object component in action.Act(entity, target)) {
                                entity.Replace(component);
                                entity.Replace(new StepInProgress());
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

                foreach (IGameplayAction action in GetActions()) {
                    if (action.ShouldAct(entity)) {
                        changed = true;
                        Debug.Log($"[Actions Queue] Executing {action.GetType().Name}");
                        foreach (object component in action.Act(entity)) {
                            entity.Replace(component);
                            entity.Replace(new StepInProgress());
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
                    Debug.Log("[Actions Queue] Complete step");
                    entity.Replace(new CompleteStep());
                }
            }
        }
    }
}
