using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.GameplayActions.Components;
using UnityEditor.SearchService;
using UnityEngine;

namespace Sources.GameplayActions {
    public class PlanActionsSystem : IEcsRunSystem {
        /// <summary>
        /// This system reacts to moved event and creates queue of gameplay actions
        /// So it's defines order of gameplay actions after dropped
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Player, PlayerMovedEvent> player;
        private EcsFilter<PlayableCard, Spawned> cards;

        public void Run() {
            // Means player not moved
            if (player.IsEmpty()) return;

            foreach (int idx in player) {
                EcsEntity entity = player.GetEntity(idx);
                ActionsQueue actionsQueue = GetQueue(player.GetEntity(idx));
                EcsEntity target = player.Get2(idx).Target;
                if (target.Has<Enemy>() && target.Has<Health>()) {
                    actionsQueue.Queue.Enqueue(new Hit { Source = target, Amount = target.Get<Health>().Amount });
                } else if (target.Has<LevelExit>()) {
                    // TODO: replace hit with level exit component
                    actionsQueue.Queue.Enqueue(new Hit { Source = target, Amount = 1 });
                } else {
                    Debug.LogWarning("Player moved, but no actions planned!");
                }

                // Finish move
                actionsQueue.Queue.Enqueue(new CompleteStep());
                entity.Replace(actionsQueue);
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
