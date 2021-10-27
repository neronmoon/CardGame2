using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using Leopotam.Ecs.UnityIntegration.Editor;
using Sources.GameplayActions.Components;
using UnityEditor;

namespace Editor {
    class ActionsQueueInspector : IEcsComponentInspector {
        Type IEcsComponentInspector.GetFieldType() {
            return typeof(ActionsQueue);
        }

        void IEcsComponentInspector.OnGUI(string label, object value, EcsWorld world, ref EcsEntity entityId) {
            ActionsQueue component = (ActionsQueue)value;
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            drawQueue(component.Queue, "Queue");
            drawQueue(component.ActiveActions, "Active");

            EditorGUI.indentLevel--;
        }

        private static void drawQueue(Queue<IGameplayTrigger> queue, string name) {
            EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.IntField("Count", queue.Count);
            foreach (IGameplayTrigger element in queue) {
                EditorGUILayout.TextField(element.GetType().ToString());
            }

            EditorGUI.indentLevel--;
        }
    }
}
