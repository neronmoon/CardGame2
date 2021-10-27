using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.GameplayActions.Components;
using UnityEngine;

namespace Sources.GameplayActions {
    public class PlanActionsSystem : IEcsRunSystem {
        /// <summary>
        /// This system reacts to moved event and creates queue of gameplay actions
        /// So it's defines order of gameplay actions after dropped
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Player, PlayerMovedEvent> player;

        public void Run() {
            foreach (int idx in player) {
                EcsEntity playerEnt = player.GetEntity(idx);
                EcsEntity target = player.Get2(idx).Target;

                Queue<IGameplayTrigger> queue = playerEnt.Has<ActionsQueue>()
                    ? playerEnt.Get<ActionsQueue>().Queue
                    : new Queue<IGameplayTrigger>();
                Queue<IGameplayTrigger> activeActions = playerEnt.Has<ActionsQueue>()
                    ? playerEnt.Get<ActionsQueue>().ActiveActions
                    : new Queue<IGameplayTrigger>();
                if (target.Has<Enemy>()) {
                    // TODO: fix amount
                    queue.Enqueue(new Hit { Source = target, Amount = 1 });
                    Debug.Log("Planned hit by enemy");
                } else if (target.Has<LevelExit>()) {
                    queue.Enqueue(new Hit { Source = target, Amount = 1 });
                    Debug.Log("Planned level exit");
                } else {
                    Debug.LogWarning("Player moved, but no actions planned!");
                }

                playerEnt.Replace(new ActionsQueue { Queue = queue, ActiveActions = activeActions});
            }
        }
    }
}
