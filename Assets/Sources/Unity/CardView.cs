using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Leopotam.Ecs;
using Sources.Database.DataObject;
using Sources.ECS;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.Components.Gameplay.Perks;
using Sources.ECS.GameplayActions.Components;
using Sources.Unity.Support;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sources.Unity {
    public class CardView : View {
        public SpriteRenderer Sprite;

        public Animator HitAnimator;

        public SpriteRenderer HighlightMask;
        public SpriteRenderer HitMask;

        public TextMeshProUGUI ValueText;
        public GameObject[] ValueObjects;

        public TextMeshProUGUI NameText;
        public GameObject[] NameObjects;
        public int AdditionalSortOrder = 0;

        public GameObject AggressivePerk;

        private Dictionary<Component, int> renderers = new(10);

        private Vector3 initHealthScale;

        private void Start() {
            initHealthScale = ValueText.transform.localScale;
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
                renderers.Add(renderer, renderer.sortingOrder);
            }

            foreach (Canvas canvas in GetComponentsInChildren<Canvas>()) {
                renderers.Add(canvas, canvas.sortingOrder);
            }

            foreach (SortingGroup group in GetComponentsInChildren<SortingGroup>()) {
                renderers.Add(group, group.sortingOrder);
            }
        }

        public void FillStats(EcsEntity entity) {
            if (entity.Has<Player>()) {
                AdditionalSortOrder = 100;
            }

            Sprite.sprite = entity.Has<Face>() ? entity.Get<Face>().Sprite : null;
            ValueText.text = MakeValueText(entity);

            foreach (GameObject o in NameObjects) {
                o.SetActive(entity.Has<Name>());
            }

            if (entity.Has<Name>()) {
                NameText.text = entity.Get<Name>().Value;
            }

            // TODO: Add icons to display item effects
            foreach (GameObject o in ValueObjects) {
                o.SetActive(ValueText.text != null);
            }

            AggressivePerk.SetActive(entity.Has<Aggressive>());
        }

        private string MakeValueText(EcsEntity entity) {
            string valueText;
            if (entity.Has<Health>()) {
                valueText = entity.Get<Health>().Value.ToString();
            } else {
                Item item = null;
                if (entity.Has<EquippableItem>()) {
                    item = entity.Get<EquippableItem>().Data;
                } else if (entity.Has<ConsumableItem>()) {
                    item = entity.Get<ConsumableItem>().Data;
                }

                if (item == null) {
                    return null;
                }
                ItemEffectsProcessor processor = new();
                object[] components = processor.ProcessItem(item, entity);

                if (components.Length > 0) {
                    int value = 0;
                    foreach (object comp in components) {
                        if (comp is Heal heal) {
                            value += heal.Amount;
                        }
                    }

                    valueText = value.ToString();
                } else {
                    valueText = item.Count.ToString();
                }

            }

            return valueText;
        }

        public void AnimateHit() {
            HitAnimator.Play("Blood");
        }

        public void AnimateHeal() {
            Transform transform = ValueText.transform;
            DOTween.Sequence()
                   .Append(transform.DOScale(initHealthScale * 1.2f, 0.2f))
                   .AppendInterval(0.1f)
                   .Append(transform.DOScale(initHealthScale, 0.2f))
                   .Play()
                ;
        }

        private void Update() {
            foreach (KeyValuePair<Component, int> x in renderers) {
                int order = (int)(x.Value + transform.position.y) + AdditionalSortOrder;
                switch (x.Key) {
                    case SpriteRenderer renderer:
                        renderer.sortingOrder = order;
                        break;
                    case Canvas renderer:
                        renderer.sortingOrder = order;
                        break;
                    case SortingGroup renderer:
                        renderer.sortingOrder = order;
                        break;
                }
            }
        }

        public void FadeIn(float time) {
            FadeOut(0f);
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
                if (!renderer.gameObject.name.Contains("Mask")) {
                    renderer.DOFade(1f, time);
                }
            }

            foreach (CanvasGroup renderer in GetComponentsInChildren<CanvasGroup>()) {
                if (!renderer.gameObject.name.Contains("Mask")) {
                    renderer.DOFade(1f, time);
                }
            }
        }

        public void FadeOut(float time, float target = 0f) {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
                renderer.DOFade(target, time);
            }

            foreach (CanvasGroup renderer in GetComponentsInChildren<CanvasGroup>()) {
                renderer.DOFade(target, time);
            }
        }
    }
}
