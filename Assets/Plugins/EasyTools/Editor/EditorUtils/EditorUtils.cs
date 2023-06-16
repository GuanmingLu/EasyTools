using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyTools.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {

	public static partial class Utils {
		public static float SingleLinePropHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		internal static void Info(Rect position, string message) => EditorGUI.HelpBox(position, message, MessageType.Info);
		internal static void Warning(Rect position, string message) => EditorGUI.HelpBox(position, message, MessageType.Warning);
		internal static void Error(Rect position, string message) => EditorGUI.HelpBox(position, message, MessageType.Error);

		internal static void InfoLayout(string message) => EditorGUILayout.HelpBox(message, MessageType.Info);
		internal static void WarningLayout(string message) => EditorGUILayout.HelpBox(message, MessageType.Warning);
		internal static void ErrorLayout(string message) => EditorGUILayout.HelpBox(message, MessageType.Error);

		public static void HorizontalLine(Rect position, float height, Color color) {
			Rect rect = EditorGUI.IndentedRect(position);
			rect.y += (EditorGUIUtility.singleLineHeight - height) / 2;
			rect.height = height;
			EditorGUI.DrawRect(rect, color);
		}

		public static void ChangedObject(UnityEngine.Object obj, string message) {
			Undo.RecordObject(obj, message);
			EditorUtility.SetDirty(obj);
		}

		public static void ChangedObjects(UnityEngine.Object[] objs, string message) {
			Undo.RecordObjects(objs, message);
			foreach (var obj in objs) EditorUtility.SetDirty(obj);
		}
	}
}
