using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {

	public static partial class Utils {
		public static SerializedPropertyType GetSerializedPropertyType(Type type) => type switch {
			_ when type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) => SerializedPropertyType.Integer,
			_ when type == typeof(bool) => SerializedPropertyType.Boolean,
			_ when type == typeof(float) || type == typeof(double) || type == typeof(decimal) => SerializedPropertyType.Float,
			_ when type == typeof(string) => SerializedPropertyType.String,
			_ when type == typeof(Color) || type == typeof(Color32) => SerializedPropertyType.Color,
			_ when type == typeof(LayerMask) => SerializedPropertyType.LayerMask,
			_ when type.IsEnum => SerializedPropertyType.Enum,
			_ when type == typeof(Vector2) => SerializedPropertyType.Vector2,
			_ when type == typeof(Vector3) => SerializedPropertyType.Vector3,
			_ when type == typeof(Vector4) => SerializedPropertyType.Vector4,
			_ when type == typeof(Rect) => SerializedPropertyType.Rect,
			_ when type == typeof(AnimationCurve) => SerializedPropertyType.AnimationCurve,
			_ when type == typeof(Bounds) => SerializedPropertyType.Bounds,
			_ when type == typeof(Quaternion) => SerializedPropertyType.Quaternion,
			_ when type == typeof(Vector2Int) => SerializedPropertyType.Vector2Int,
			_ when type == typeof(Vector3Int) => SerializedPropertyType.Vector3Int,
			_ when type == typeof(RectInt) => SerializedPropertyType.RectInt,
			_ when type == typeof(BoundsInt) => SerializedPropertyType.BoundsInt,
			_ when type == typeof(Gradient) => SerializedPropertyType.Gradient,
			_ when typeof(UnityEngine.Object).IsAssignableFrom(type) => SerializedPropertyType.ObjectReference,
			_ => SerializedPropertyType.Generic,
		};
		public static float GetPropertyHeight(Type type, GUIContent label) => EditorGUI.GetPropertyHeight(GetSerializedPropertyType(type), label);

		private static T Do<T>(Func<T> func) => func();

		public static object ValueField(Rect position, string label, object value, Type valueType = null) {
			if (valueType != null) {
				if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
					return EditorGUI.ObjectField(position, label, value as UnityEngine.Object, valueType, true);
				if (value is null) {
					try {
						value = Activator.CreateInstance(valueType);
					}
					catch { }
				}
				if (TryGetPropDrawer(valueType, out var drawer)) {
					drawer.OnGUI(position, value, label);
					return value;
				}
			}

			return value switch {
				null => new Func<object>(() => { EditorGUI.LabelField(position, label, "null"); return value; }).Invoke(),
				Enum e => EditorGUI.EnumPopup(position, label, e),
				bool b => EditorGUI.Toggle(position, label, b),
				byte or sbyte or short or ushort or int or uint => EditorGUI.IntField(position, label, (int)value),
				long or ulong => EditorGUI.LongField(position, label, (long)value),
				float or double or decimal => EditorGUI.FloatField(position, label, (float)value),
				string s => EditorGUI.TextField(position, label, s),
				Color c => EditorGUI.ColorField(position, label, c),
				Color32 c32 => EditorGUI.ColorField(position, label, c32),
				LayerMask l => EditorGUI.LayerField(position, label, l),
				Vector2 v2 => EditorGUI.Vector2Field(position, label, v2),
				Vector3 v3 => EditorGUI.Vector3Field(position, label, v3),
				Vector4 v4 => EditorGUI.Vector4Field(position, label, v4),
				Rect r => EditorGUI.RectField(position, label, r),
				AnimationCurve ac => EditorGUI.CurveField(position, label, ac),
				Bounds b => EditorGUI.BoundsField(position, label, b),
				Quaternion q => EditorGUI.Vector3Field(position, label, q.eulerAngles),
				Vector2Int v2i => EditorGUI.Vector2IntField(position, label, v2i),
				Vector3Int v3i => EditorGUI.Vector3IntField(position, label, v3i),
				RectInt ri => EditorGUI.RectIntField(position, label, ri),
				BoundsInt bi => EditorGUI.BoundsIntField(position, label, bi),
				Gradient g => EditorGUI.GradientField(position, label, g),
				_ => Do(() => { EditorGUI.LabelField(position, label, value?.ToString() ?? "null"); return value; }),
			};
		}

		public static object ValueFieldLayout(string label, object value, Type valueType = null) {
			if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
				return EditorGUILayout.ObjectField(label, value as UnityEngine.Object, valueType, true);

			if (value is null && valueType != null) {
				try {
					value = Activator.CreateInstance(valueType);
				}
				catch { }
			}

			return value switch {
				Enum e => EditorGUILayout.EnumPopup(label, e),
				bool b => EditorGUILayout.Toggle(label, b),
				byte or sbyte or short or ushort or int or uint => EditorGUILayout.IntField(label, (int)value),
				long or ulong => EditorGUILayout.LongField(label, (long)value),
				float or double or decimal => EditorGUILayout.FloatField(label, (float)value),
				string s => EditorGUILayout.TextField(label, s),
				Color c => EditorGUILayout.ColorField(label, c),
				Color32 c32 => EditorGUILayout.ColorField(label, c32),
				LayerMask l => EditorGUILayout.LayerField(label, l),
				Vector2 v2 => EditorGUILayout.Vector2Field(label, v2),
				Vector3 v3 => EditorGUILayout.Vector3Field(label, v3),
				Vector4 v4 => EditorGUILayout.Vector4Field(label, v4),
				Rect r => EditorGUILayout.RectField(label, r),
				AnimationCurve ac => EditorGUILayout.CurveField(label, ac),
				Bounds b => EditorGUILayout.BoundsField(label, b),
				Quaternion q => EditorGUILayout.Vector3Field(label, q.eulerAngles),
				Vector2Int v2i => EditorGUILayout.Vector2IntField(label, v2i),
				Vector3Int v3i => EditorGUILayout.Vector3IntField(label, v3i),
				RectInt ri => EditorGUILayout.RectIntField(label, ri),
				BoundsInt bi => EditorGUILayout.BoundsIntField(label, bi),
				Gradient g => EditorGUILayout.GradientField(label, g),
				_ => Do(() => { EditorGUILayout.LabelField(label, value?.ToString() ?? "null"); return value; }),
			};
		}
	}
}
