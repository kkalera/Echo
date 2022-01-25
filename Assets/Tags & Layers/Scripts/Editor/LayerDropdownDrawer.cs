using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TagsAndLayers.Components
{
    [CustomPropertyDrawer(typeof(LayerDropdownAttribute))]
    public class LayerDropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                Type propertyType = property.GetValueType();

                if (propertyType == typeof(string))
                    RenderFromString(position, property, label);
                else if (propertyType == typeof(int))
                    RenderFromInteger(position, property, label);
                else
                    throw new Exception($"{propertyType} not supported!");
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                EditorGUI.PropertyField(
                    position,
                    property,
                    label);
            }
        }

        private void RenderFromInteger(Rect position, SerializedProperty property, GUIContent label)
        {
            string layername = LayerMask.LayerToName(property.intValue);

            int currentLayerIndex = Array.IndexOf(
                InternalEditorUtility.layers,
                layername);

            int newLayerIndex = GetNewLayerIndex(currentLayerIndex, position, label);
            property.intValue = LayerMask.NameToLayer(InternalEditorUtility.layers[newLayerIndex]);
        }

        private void RenderFromString(Rect position, SerializedProperty property, GUIContent label)
        {
            int currentLayerIndex = Array.IndexOf(
                InternalEditorUtility.layers,
                property.stringValue);

            int newLayerIndex = GetNewLayerIndex(currentLayerIndex, position, label);
            property.stringValue = InternalEditorUtility.layers[newLayerIndex];
        }

        private int GetNewLayerIndex(int currentLayerIndex, Rect position, GUIContent label)
        {
            if (currentLayerIndex >= 0)
            {
                return EditorGUI.Popup(
                    position,
                    label.text,
                    currentLayerIndex,
                    InternalEditorUtility.layers);
            }
            else
            {
                return 0;
            }
        }
    }
}
