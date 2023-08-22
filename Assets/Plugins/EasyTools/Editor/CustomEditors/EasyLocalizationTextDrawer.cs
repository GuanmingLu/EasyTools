using System;
using System.Linq;
using EasyTools.InternalComponent;
using EasyTools.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {

	[CanEditMultipleObjects]
	[CustomEditor(typeof(EasyLocalizationText))]
	public class EasyLocalizationTextDrawer : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			serializedObject.Update();
			var t = target as EasyLocalizationText;
			var targetProperty = serializedObject.FindProperty("target");

			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(targetProperty, new GUIContent(""));
			if (EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
				t.Refresh();
			}

			var options = targetProperty.objectReferenceValue != null
				? targetProperty.objectReferenceValue.Reflect().GetSettableValues<string>().Select(v => v.DisplayName).ToArray() : new string[0];
			var idx = Array.IndexOf(options, t.propertyName);

			EditorGUI.BeginChangeCheck();
			idx = EditorGUILayout.Popup(idx, options);
			if (EditorGUI.EndChangeCheck() || idx == -1) {
				serializedObject.FindProperty("propertyName").stringValue = idx == -1 ? null : options[idx];
				serializedObject.ApplyModifiedProperties();
				t.Refresh();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("fileName"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("key"));
			if (EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
				t.Refresh();
			}

			if (GUILayout.Button("刷新")) {
				t.Refresh();
			}
		}
	}
}
