using System;
using Leopotam.Ecs;
using Leopotam.Ecs.UnityIntegration.Editor;
using Sources.Database.DataObject;
using Sources.ECS.Components.Gameplay;
using UnityEditor;

namespace Editor {
    class InventoryInspector : IEcsComponentInspector {
        Type IEcsComponentInspector.GetFieldType() {
            return typeof(Inventory);
        }

        void IEcsComponentInspector.OnGUI(string label, object value, EcsWorld world, ref EcsEntity entityId) {
            Inventory component = (Inventory)value;
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            foreach ((Item item, int count) in component.Items) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(item.ToString());
                EditorGUILayout.LabelField(count.ToString());
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
    }
}
