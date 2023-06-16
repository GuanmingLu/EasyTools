using UnityEngine;
using UnityEditor;
using EasyTools.Settings;
using System;
using System.Linq;

namespace EasyTools.Editor {
	public class StaticSerializeFieldEditorWindow : EditorWindow {
		[MenuItem("EasyTools/Static SerializeField")]
		public static void Open() {
			GetWindow<StaticSerializeFieldEditorWindow>();
		}

		private void OnEnable() {
			titleContent = new GUIContent("Static SerializeField");
		}

		private void OnGUI() {
		}
	}
}
