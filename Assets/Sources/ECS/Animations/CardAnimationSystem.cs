using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Database.DataObject;
using Sources.ECS.Animations.Components;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.Components.Processes;
using Sources.ECS.Extensions;
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

        private SceneData sceneData;
        private RuntimeData runtimeData;
        private EcsFilter<LevelIsChanging> levelIsChanging;
        private EcsFilter<PlayableCard, VisualObject, LevelPosition> cards;
        private EcsFilter<PlayableCard, LevelPosition, Player> playerCard;

        private Random random = new();

        public void Run() {
            foreach (int idx in cards) {
                LevelPosition playerPosition = playerCard.GetComponentOnFirstOrDefault(new LevelPosition { Y = 0, X = 1 });
                EcsEntity entity = cards.GetEntity(idx);
                GameObject obj = entity.Get<VisualObject>().Object;
                if (obj == null) continue;

                Transform transform = obj.transform;
                if (!entity.Has<CardAnimationState>()) {
                    entity.Replace(new CardAnimationState {
                        InitScale = transform.localScale,
                        Components = new List<Type>(),
                        SpawnAnimationStarted = false,
                        SpawnAnimationCompleted = false,
                    });
                }

                CardAnimationState state = entity.Get<CardAnimationState>();
                CardView view = obj.GetComponent<CardView>();
                animate<Dragging>(entity, (up) => transform.DOScale(state.InitScale * (up ? 1.1f : 1f), 0.3f), false);
                animate<DropCandidate>(entity, (up) => view.HighlightMask.DOFade(up ? 0.5f : 0f, 0.1f));
                animate<Spawned>(
                    entity,
                    (up) => {
                        if (!up) return;
                        LevelPosition levelPosition = entity.Get<LevelPosition>();
                        Vector3 targetPos = calcLevelPosition(levelPosition);

                        transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                        if (entity.Has<EnemyDrop>()) {
                            transform.position = new Vector3(targetPos.x, targetPos.y + 2, targetPos.z);
                            transform.DOMove(new Vector3(targetPos.x, targetPos.y, targetPos.z), 0.5f);
                            view.FadeIn(0.5f);
                            return;
                        }

                        int maxSpawnedY = 0;
                        foreach (int idx in cards) {
                            EcsEntity card = cards.GetEntity(idx);
                            if (card.Has<Discarded>()) continue;
                            if (card.Has<CardAnimationState>() && card.Get<CardAnimationState>().SpawnAnimationCompleted) {
                                maxSpawnedY = Math.Max(cards.Get3(idx).Y, maxSpawnedY);
                            }
                        }

                        int levelWidth = runtimeData.CurrentLevel.Width;
                        float rawDelay = (Math.Abs(Math.Max(playerPosition.Y, maxSpawnedY) - levelPosition.Y) + 1f) * levelWidth -
                                         (levelWidth - (levelPosition.X + 0.5f));

                        float delay = Mathf.Max(0f, rawDelay * 0.1f);
                        const float time = 0.8f;

                        DOTween.Sequence()
                               .AppendInterval(delay)
                               .AppendCallback(() => {
                                   CardAnimationState state = entity.Get<CardAnimationState>();
                                   state.SpawnAnimationStarted = true;
                                   entity.Replace(state);
                               })
                               .Append(transform.DOBlendableMoveBy(calcLevelPosition(levelPosition) - transform.position, time).SetEase(Ease.OutCubic))
                               .AppendCallback(() => {
                                   CardAnimationState state = entity.Get<CardAnimationState>();
                                   state.SpawnAnimationStarted = false;
                                   state.SpawnAnimationCompleted = true;
                                   entity.Replace(state);
                               })
                               .Play();

                        transform.DORotate(new Vector3(0f, 0f, randomFloat(-2.5f, 2.5f)), time);
                    },
                    false
                );
                animate<Discarded>(entity, (up) => view.FadeOut(0.2f));
                animate<PreHit>(entity, (up) => {
                    if (!up) return;
                    Hit hit = entity.Get<PreHit>().Hit;
                    EcsEntity source = hit.Source;
                    GameObject enemyObj = source.Get<VisualObject>().Object;
                    enemyObj.GetComponent<CardView>().AdditionalSortOrder += 200;
                    Transform enemyTransform = enemyObj.transform;
                    enemyTransform.DOScale(Vector3.one * 1.1f, 0.4f);
                    enemyTransform.DOMove(transform.position + Vector3.one / 3, 0.3f);
                });
                animate<Hit>(entity, (up) => {
                    if (!up) return;
                    view.AnimateHit();
                    DOTween.Sequence()
                           .Append(view.HitMask.DOFade(0.5f, 0.2f))
                           .Append(view.HitMask.DOFade(0f, 0.2f))
                           .Play();
                    transform.DOPunchScale(-Vector3.one / 5, 0.4f, 1);
                });
                animate<Heal>(entity, _ => view.AnimateHeal());
                animate<Dead>(entity, (up) => {
                    if (!entity.Has<Player>()) return;

                    sceneData.DeathScreenView.Show();
                    transform.DOMove(sceneData.OriginPoint.transform.position, 0.3f);
                });
                animate<LevelChange>(entity, (up) => {
                    if (up) return;
                    transform.DOMove(calcLevelPosition(entity.Get<LevelPosition>()), 0.5f).SetDelay(0.3f);
                });

                animate<CompleteStep>(entity, (up) => {
                    // Do not move any card if player become dead! This will cause animation conflicts!
                    if (!up || entity.Has<Dead>()) return;

                    // Only player completed step, but we move all cards  
                    foreach (int i in cards) {
                        EcsEntity cardEntity = cards.GetEntity(i);
                        GameObject cardObject = cardEntity.Get<VisualObject>().Object;
                        LevelPosition pos = cardEntity.Get<LevelPosition>();
                        cardObject.transform.DOMove(calcLevelPosition(pos), 0.5f);
                    }
                });
            }
        }

        private Vector3 calcLevelPosition(LevelPosition position) {
            Vector2 origin = sceneData.OriginPoint.position;
            int relativeX = position.X - Mathf.FloorToInt(runtimeData.CurrentLevel.Width / 2);
            return new Vector3(
                origin.x + sceneData.CardSpacing.x * relativeX,
                origin.y + sceneData.CardSpacing.y * (position.Y - GetPlayerPosition().Y),
                0
            );
        }

        private LevelPosition GetPlayerPosition() {
            return playerCard.GetComponentOnFirstOrDefault(new LevelPosition { Y = 0, X = 1 });
        }

        private float randomFloat(float min, float max) {
            float range = max - min;
            double sample = random.NextDouble();
            return (float)(sample * range + min);
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
            if (!entity.Has<CardAnimationState>()) return;
            if (currentState == lastState) {
                return;
            }

            entity.Replace(updateState.Invoke(currentState));
            entity.Replace(new Animated { Blocking = block });

            transition.Invoke(currentState);
        }
    }
}
