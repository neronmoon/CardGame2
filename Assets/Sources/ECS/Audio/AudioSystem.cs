using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Animations.Components;
using Sources.ECS.Audio.Components;
using Sources.ECS.Components;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;
using Random = System.Random;

namespace Sources.ECS.Audio {
    public class AudioSystem : IEcsRunSystem {
        /// <summary>
        /// This system reacts to events and plays according clips
        /// </summary>
        private EcsWorld world;

        private SceneData sceneData;
        private Configuration configuration;
        private EcsFilter<PlayableCard, VisualObject> cards;
        private Random random = new();

        public void Run() {
            AudioSource source = sceneData.SFXAudioSource;

            foreach (int idx in cards) {
                EcsEntity entity = cards.GetEntity(idx);
                if (!entity.Has<SoundState>()) {
                    entity.Replace(new SoundState {
                        Components = new List<Type>(),
                    });
                }

                if (entity.Has<CardAnimationState>() && entity.Get<CardAnimationState>().SpawnAnimationStarted) {
                    play<Spawned>(entity, _ => source.PlayOneShot(configuration.CardSpawnClip));
                }

                play<Hit>(entity, up => {
                    if (up) {
                        source.PlayOneShot(randomClip(configuration.HitClips));
                    }
                });
                play<Heal>(entity, _ => source.PlayOneShot(configuration.PotionClip));
                play<Dead>(entity, _ => source.PlayOneShot(configuration.DeadClip));
            }
        }

        private static void play<T>(EcsEntity entity, Action<bool> action) where T : struct {
            SoundState state = entity.Get<SoundState>();
            Type type = typeof(T);
            bool lastState = state.Components.Contains(type);
            bool newState = entity.Has<T>();

            if (lastState == newState) {
                return;
            }

            if (newState) {
                state.Components.Add(type);
            } else {
                state.Components.Remove(type);
            }

            entity.Replace(state);
            action.Invoke(newState);
        }

        private AudioClip randomClip(AudioClip[] clips) {
            return clips[random.Next(0, clips.Length)];
        }
    }
}
