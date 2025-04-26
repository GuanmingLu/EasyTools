using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using UObj = UnityEngine.Object;

namespace EasyTools.Editor {

	[CustomPropertyDrawer(typeof(StaticReferences))]
	public class StaticReferencesDrawer : PropertyDrawer {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float propertyHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			if (property.isExpanded) {
				var target = fieldInfo.GetValue(property.serializedObject.targetObject) as StaticReferences;
				foreach (var (field, _) in target.Dict) {
					propertyHeight += EditorGUI.GetPropertyHeight(Utils.GetSerializedPropertyType(field.FieldType), label) + EditorGUIUtility.standardVerticalSpacing;
				}
			}
			return propertyHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			label = EditorGUI.BeginProperty(position, label, property);
			var labelPosition = position;
			labelPosition.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(labelPosition, property, label, false);
			if (property.isExpanded) {
				EditorGUI.indentLevel++;
				var linePosition = position;
				linePosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

				var target = fieldInfo.GetValue(property.serializedObject.targetObject) as StaticReferences;
				Dictionary<FieldInfo, object> changes = new();
				foreach (var (field, obj) in target.Dict) {
					var type = field.FieldType;
					linePosition.height = EditorGUI.GetPropertyHeight(Utils.GetSerializedPropertyType(type), label);
					var newValue = Utils.ValueField(linePosition, new GUIContent(field.Name, field.DeclaringType.FullName + "." + field.Name), obj, type);
					if (!Equals(newValue, obj)) changes[field] = newValue;
					linePosition.y += linePosition.height + EditorGUIUtility.standardVerticalSpacing;
				}
				if (changes.Count > 0) {
					foreach (var (field, obj) in changes) target.Dict[field] = obj;
					Utils.ChangedObject(property.serializedObject.targetObject, "Changed StaticReferences");
				}

				EditorGUI.indentLevel--;
			}
			EditorGUI.EndProperty();
		}
	}
}
