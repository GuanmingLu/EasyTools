using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {
	[CustomEditor(typeof(RectTransform))]
	public class RectTransformEditor : InternalEditor {
		private bool showExtended;
		private SerializedProperty sizeDelta;

		public RectTransformEditor() : base("RectTransformEditor") { }

		private void OnEnable() {
			showExtended = EditorPrefs.GetBool("RectTransformEditor.showExtendedProperties", false);
			sizeDelta = serializedObject.FindProperty("m_SizeDelta");
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			EditorGUI.BeginChangeCheck();
			showExtended = EditorGUILayout.Foldout(showExtended, "Extended Options", true);
			if (EditorGUI.EndChangeCheck()) EditorPrefs.SetBool("RectTransformEditor.showExtendedProperties", showExtended);

			if (showExtended) {
				EditorGUI.indentLevel++;
				serializedObject.Update();
				var oldSize = sizeDelta.vector2Value;
				var newSize = EditorGUILayout.Vector2Field("Locked Size", oldSize);
				if (newSize.x != oldSize.x) {
					newSize.y *= newSize.x / oldSize.x;
					sizeDelta.vector2Value = newSize;
					serializedObject.ApplyModifiedProperties();
				}
				else if (newSize.y != oldSize.y) {
					newSize.x *= newSize.y / oldSize.y;
					sizeDelta.vector2Value = newSize;
					serializedObject.ApplyModifiedProperties();
				}
				EditorGUI.indentLevel--;
			}
		}
	}
}
