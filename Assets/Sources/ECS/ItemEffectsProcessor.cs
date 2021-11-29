using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Database.DataObject;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;

namespace Sources.ECS {
    public class ItemEffectsProcessor {
        public object[] ProcessItem(Item item, EcsEntity entity) {
            int healAmount = 0;

            foreach (ItemEffect effect in item.Effects) {
                switch (effect.Name) {
                    case ItemEffectType.Resurrection:
                        if (entity.Get<Health>().Value <= 0) {
                            healAmount += (int)effect.Value;
                        }

                        break;
                    case ItemEffectType.Heal:
                        healAmount += (int)effect.Value;
                        break;
                    default:
                        Debug.LogWarning($"Not applied item effect {effect.Name}");
                        break;
                }
            }

            List<object> components = new(1);
            if (healAmount > 0) {
                components.Add(new Heal { Amount = healAmount });
            }

            return components.ToArray();
        }
    }
}
