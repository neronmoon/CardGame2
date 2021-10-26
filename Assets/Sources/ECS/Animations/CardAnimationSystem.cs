using System;
using System.Collections.Generic;
using DG.Tweening;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Animations.Components;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using UnityEngine;

namespace Sources.ECS.Animations {
    public class CardAnimationSystem : IEcsRunSystem {
        /// <summary>
        /// </summary>
        private EcsWorld world;

        private EcsFilter<PlayableCard, VisualObject> cards;
        private SceneData sceneData;
        private RuntimeData runtimeData;
        private EcsFilter<PlayableCard, Player, LevelPosition> playerCard;

        private EcsSystems systems;

        public void Run() {
            foreach (var idx in cards) {
                EcsEntity entity = cards.GetEntity(idx);
                GameObject obj = entity.Get<VisualObject>().Object;
                if (!entity.Has<CardAnimationState>()) {
                    entity.Replace(new CardAnimationState {
                        InitScale = obj.transform.localScale,
                        Components = new List<Type>()
                    });
                }

                CardAnimationState state = entity.Get<CardAnimationState>();
                animate<Dragging>(entity, (up) => seq(obj.transform.DOScale(state.InitScale * (up ? 1.1f : 1f), 0.3f)));
                if (entity.Has<LevelPosition>()) {
                    animate<Spawned>(
                        entity,
                        (up) => {
                            LevelPosition levelPosition = entity.Get<LevelPosition>();
                            Vector3 targetPos = calcLevelPosition(levelPosition);
                            obj.transform.position = new Vector3(targetPos.x, obj.transform.position.y, targetPos.z);
                            float delay = 0.3f * levelPosition.X + (levelPosition.Y - runtimeData.PlayerPosition.Y) * 0.3f;
                            return seq(obj.transform.DOMove(targetPos, 1.2f)).SetDelay(delay).SetEase(Ease.OutCubic);
                        }
                    );
                }
            }
        }

        private Vector3 calcLevelPosition(LevelPosition position) {
            Vector2 origin = sceneData.OriginPoint.position;
            int relativeX = position.X - Mathf.FloorToInt(runtimeData.CurrentLevel.Width / 2);
            return new Vector3(
                origin.x + sceneData.CardSpacing.x * relativeX,
                origin.y + sceneData.CardSpacing.y * position.Y,
                0
            );
        }

        private Sequence seq(Tween tween) {
            return DOTween.Sequence().Append(tween);
        }

        private void animate<T>(EcsEntity entity, Func<bool, Sequence> transition) where T : struct {
            CardAnimationState state = entity.Get<CardAnimationState>();
            Type type = typeof(T);
            animate(entity, entity.Has<T>(), state.Components.Contains(type), (up) => {
                if (up) {
                    state.Components.Add(type);
                } else {
                    state.Components.Remove(type);
                }

                return state;
            }, transition);
        }

        private void animate(
            EcsEntity entity,
            bool currentState,
            bool lastState,
            Func<bool, CardAnimationState> updateState,
            Func<bool, Sequence> transition
        ) {
            if (currentState == lastState) {
                return;
            }

            entity.Replace(updateState.Invoke(currentState));

            transition.Invoke(currentState).Play();
        }
    }
}
