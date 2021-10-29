using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Animations.Components;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;
using Sources.Unity;
using UnityEngine;
using Random = System.Random;

namespace Sources.ECS.Animations {
    public class CardAnimationSystem : IEcsRunSystem {
        /// <summary>
        /// This is system, responsible for starting card animations.
        /// </summary>
        private EcsWorld world;

        private EcsFilter<PlayableCard, VisualObject> cards;
        private SceneData sceneData;
        private RuntimeData runtimeData;
        private EcsFilter<PlayableCard, Player, LevelPosition> playerCard;

        private Random random = new Random();

        public void Run() {
            foreach (int idx in cards) {
                EcsEntity entity = cards.GetEntity(idx);
                GameObject obj = entity.Get<VisualObject>().Object;
                Transform transform = obj.transform;
                if (!entity.Has<CardAnimationState>()) {
                    entity.Replace(new CardAnimationState {
                        InitScale = transform.localScale,
                        Components = new List<Type>()
                    });
                }

                CardAnimationState state = entity.Get<CardAnimationState>();
                animate<Dragging>(entity, (up) => transform.DOScale(state.InitScale * (up ? 1.1f : 1f), 0.3f), false);
                animate<DropCandidate>(
                    entity,
                    (up) => obj.GetComponent<CardView>().HighlightMask.DOFade(up ? 0.5f : 0f, 0.1f)
                );
                if (entity.Has<LevelPosition>()) {
                    animate<Spawned>(
                        entity,
                        (up) => {
                            LevelPosition levelPosition = entity.Get<LevelPosition>();
                            Vector3 targetPos = calcLevelPosition(levelPosition);
                            transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                            int nulls = runtimeData.LevelLayout[levelPosition.Y].Count((x) => x == null);
                            float delay = 0.3f * levelPosition.X +
                                          (levelPosition.Y - runtimeData.PlayerPosition.Y - nulls) * 0.1f;
                            const float time = 0.9f;
                            transform.DOMove(targetPos, time).SetDelay(delay).SetEase(Ease.OutCubic);
                            transform.DORotate(new Vector3(0f, 0f, randomFloat(-2.5f, 2.5f)), time);
                        },
                        false
                    );
                }

                animate<Discarded>(entity, (up) => {
                    const float fadeTime = 0.2f;
                    foreach (SpriteRenderer renderer in obj.GetComponentsInChildren<SpriteRenderer>()) {
                        renderer.DOFade(0f, fadeTime);
                    }

                    foreach (CanvasGroup renderer in obj.GetComponentsInChildren<CanvasGroup>()) {
                        renderer.DOFade(0f, fadeTime);
                    }
                });

                animate<Hit>(entity, (up) => { transform.DOPunchScale(-Vector3.one / 5, 0.2f, 1); });

                animate<CompleteStep>(entity, (up) => {
                    // Only player completed step, but we move all cards  
                    foreach (int i in cards) {
                        EcsEntity cardEntity = cards.GetEntity(i);
                        Vector3 targetPos = calcLevelPosition(cardEntity.Get<LevelPosition>());
                        GameObject gameObject = cardEntity.Get<VisualObject>().Object;
                        // This animation conflicting with non-blocking Spawned animation cause moving artifacts
                        // But this animations does same thing - moves cards to right position
                        // So if gameobject is already tweening — this animation don't need to do same thing
                        if (!DOTween.IsTweening(gameObject.transform)) {
                            gameObject.transform.DOMove(targetPos, 0.5f);
                        }
                    }
                });
            }
        }

        private Vector3 calcLevelPosition(LevelPosition position) {
            Vector2 origin = sceneData.OriginPoint.position;
            int relativeX = position.X - Mathf.FloorToInt(runtimeData.CurrentLevel.Width / 2);
            return new Vector3(
                origin.x + sceneData.CardSpacing.x * relativeX,
                origin.y + sceneData.CardSpacing.y * (position.Y - runtimeData.PlayerPosition.Y),
                0
            );
        }

        private float randomFloat(float min, float max) {
            float range = max - min;
            double sample = random.NextDouble();
            return (float) (sample * range + min);
        }

        private static void animate<T>(EcsEntity entity, Action<bool> transition, bool block = true) where T : struct {
            CardAnimationState state = entity.Get<CardAnimationState>();
            Type type = typeof(T);
            animate(entity, entity.Has<T>(), state.Components.Contains(type), (up) => {
                if (up) {
                    state.Components.Add(type);
                } else {
                    state.Components.Remove(type);
                }

                return state;
            }, transition, block);
        }

        private static void animate(
            EcsEntity entity,
            bool currentState,
            bool lastState,
            Func<bool, CardAnimationState> updateState,
            Action<bool> transition,
            bool block = true
        ) {
            if (currentState == lastState) {
                return;
            }

            entity.Replace(updateState.Invoke(currentState));
            entity.Replace(new Animated {Blocking = block});

            transition.Invoke(currentState);
        }
    }
}
