using System;
using System.Reflection;
using UnityEditor;

namespace TagsAndLayers.Components
{
    public static class SerializedPropertyExtensions
    {
        public static Type GetValueType(this SerializedProperty property)
        {
            Type parentType = property.serializedObject.targetObject.GetType();
            FieldInfo field = parentType.GetField(
                property.propertyPath,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            return field.FieldType;
        }
    }
}
