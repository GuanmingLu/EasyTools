using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EasyTools.Editor {

	[CustomEditor(typeof(InternalComponent.Comment))]
	public class CommentDrawer : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			serializedObject.Update();
			var prop = serializedObject.FindProperty("m_comment");
			EditorGUI.BeginChangeCheck();
			var msg = EditorGUILayout.TextArea(prop.stringValue);
			if (EditorGUI.EndChangeCheck()) {
				prop.stringValue = msg;
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
