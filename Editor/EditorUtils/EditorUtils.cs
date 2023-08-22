using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

		public static void BeginBoxGroup(string title, float spacing = 2) {
			GUILayout.Space(spacing);

			EditorGUILayout.BeginVertical(GUI.skin.box);
			if (!string.IsNullOrEmpty(title)) {
				EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
			}
		}

		public static void EndBoxGroup(float spacing = 2) {
			EditorGUILayout.EndVertical();

			GUILayout.Space(spacing);
		}

		public static string GetAssetFullPath(UnityEngine.Object obj) => Path.GetFullPath(AssetDatabase.GetAssetPath(obj));

		public static void ChooseProjectPath(ref string path) {
			path = EditorGUILayout.TextArea(string.IsNullOrWhiteSpace(path) ? "请输入路径" : path);

			if (GUILayout.Button("使用当前路径")) {
				if (Selection.assetGUIDs.Length == 1) {
					var p = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
					// 只有以 Assets 开头的路径才合法
					if (p.StartsWith("Assets")) {
						// 如果有扩展名（选中的是文件）则选择其文件夹
						path = Path.HasExtension(p) ? Path.GetDirectoryName(p).Replace('\\', '/') : p;
					}
				}
			}
		}

		public struct BoxGroupScope : IDisposable {
			private float _spacing;

			public BoxGroupScope(string title, float spacing = 2) {
				_spacing = spacing;

				GUILayout.Space(_spacing);

				EditorGUILayout.BeginVertical(GUI.skin.box);
				if (!string.IsNullOrEmpty(title)) {
					EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
				}
			}

			public void Dispose() {
				EditorGUILayout.EndVertical();

				GUILayout.Space(_spacing);
			}
		}
	}
}
