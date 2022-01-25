using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace TagsAndLayers.Components
{
    [CustomPropertyDrawer(typeof(TagDropdownAttribute))]
    public class TagDropdownDrawer : PropertyDrawer
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
            property.intValue = GetNewTagIndex(property.intValue, position, label);
        }

        private void RenderFromString(Rect position, SerializedProperty property, GUIContent label)
        {
            int currentTabIndex = Array.IndexOf(
                InternalEditorUtility.tags,
                property.stringValue);

            int newTabIndex = GetNewTagIndex(currentTabIndex, position, label);
            property.stringValue = InternalEditorUtility.tags[newTabIndex];
        }

        private int GetNewTagIndex(int currentTagIndex, Rect position, GUIContent label)
        {
            if (currentTagIndex >= 0)
            {
                return EditorGUI.Popup(
                    position,
                    label.text,
                    currentTagIndex,
                    InternalEditorUtility.tags);
            }
            else
            {
                return 0;
            }
        }
    }
}
