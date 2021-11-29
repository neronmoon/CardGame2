using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Database.DataObject;
using Sources.ECS.Components.Gameplay;
using UnityEngine;

namespace Sources.ECS.GameplayActions.Actions {
    public class ResurrectAction : IGameplayAction {
        public bool ShouldAct(EcsEntity entity) => entity.Has<Health>() &&
                                                   entity.Get<Health>().Value <= 0 &&
                                                   entity.Get<Inventory>().HasItemWithEffect(ItemEffectType.Resurrection);

        public object[] Act(EcsEntity entity) {
            Inventory inventory = entity.Get<Inventory>();
            Item item = inventory.TakeFirstItemWithEffect(ItemEffectType.Resurrection);

            List<object> components = new() { inventory };

            foreach (ItemEffect effect in item.Effects) {
                switch (effect.Name) {
                    case ItemEffectType.Resurrection:
                        components.Add(new Health { Value = (int)effect.Value });
                        break;
                    default:
                        Debug.LogWarning($"Not applied item effect {effect.Name}");
                        break;
                }
            }

            return components.ToArray();
        }
    }
}
