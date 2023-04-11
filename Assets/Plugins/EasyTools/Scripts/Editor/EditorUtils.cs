using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyTools.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {

	public static class Utils {
		public static float SingleLinePropHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		internal static void Info(Rect position, string message) => EditorGUI.HelpBox(position, message, MessageType.Info);
		internal static void Warning(Rect position, string message) => EditorGUI.HelpBox(position, message, MessageType.Warning);
		internal static void Error(Rect position, string message) => EditorGUI.HelpBox(position, message, MessageType.Error);

		internal static void InfoLayout(string message) => EditorGUILayout.HelpBox(message, MessageType.Info);
		internal static void WarningLayout(string message) => EditorGUILayout.HelpBox(message, MessageType.Warning);
		internal static void ErrorLayout(string message) => EditorGUILayout.HelpBox(message, MessageType.Error);

		private static Dictionary<Type, SerializedPropertyType> SerializedPropertyTypeDict = new Dictionary<Type, SerializedPropertyType>(){
			{   typeof(byte),           SerializedPropertyType.Integer              },
			{   typeof(sbyte),          SerializedPropertyType.Integer              },
			{   typeof(short),          SerializedPropertyType.Integer              },
			{   typeof(ushort),         SerializedPropertyType.Integer              },
			{   typeof(int),            SerializedPropertyType.Integer              },
			{   typeof(uint),           SerializedPropertyType.Integer              },
			{   typeof(long),           SerializedPropertyType.Integer              },
			{   typeof(ulong),          SerializedPropertyType.Integer              },
			{   typeof(bool),           SerializedPropertyType.Boolean              },
			{   typeof(float),          SerializedPropertyType.Float                },
			{   typeof(double),         SerializedPropertyType.Float                },
			{   typeof(decimal),        SerializedPropertyType.Float                },
			{   typeof(string),         SerializedPropertyType.String               },
			{   typeof(Color),          SerializedPropertyType.Color                },
			{   typeof(Color32),        SerializedPropertyType.Color                },
			{   typeof(LayerMask),      SerializedPropertyType.LayerMask            },
			{   typeof(Enum),           SerializedPropertyType.Enum                 },
			{   typeof(Vector2),        SerializedPropertyType.Vector2              },
			{   typeof(Vector3),        SerializedPropertyType.Vector3              },
			{   typeof(Vector4),        SerializedPropertyType.Vector4              },
			{   typeof(Rect),           SerializedPropertyType.Rect                 },
			{   typeof(AnimationCurve), SerializedPropertyType.AnimationCurve       },
			{   typeof(Bounds),         SerializedPropertyType.Bounds               },
			{   typeof(Quaternion),     SerializedPropertyType.Quaternion           },
			{   typeof(Vector2Int),     SerializedPropertyType.Vector2Int           },
			{   typeof(Vector3Int),     SerializedPropertyType.Vector3Int           },
			{   typeof(RectInt),        SerializedPropertyType.RectInt              },
			{   typeof(BoundsInt),      SerializedPropertyType.BoundsInt            },
			{   typeof(Gradient),       SerializedPropertyType.Gradient },
		};
		public static SerializedPropertyType GetSerializedPropertyType(Type type) {
			while (type != null) {
				if (SerializedPropertyTypeDict.TryGetValue(type, out var serializedPropertyType)) {
					return serializedPropertyType;
				}
				type = type.BaseType;
			}
			return SerializedPropertyType.Generic;
		}
		public static float GetPropertyHeight(Type type, GUIContent label) => EditorGUI.GetPropertyHeight(GetSerializedPropertyType(type), label);


		public static object ValueField(Rect position, string label, object value, Type valueType) {
			if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
				return EditorGUI.ObjectField(position, label, value as UnityEngine.Object, valueType, true);
			if (valueType == typeof(byte) || valueType == typeof(sbyte) || valueType == typeof(short) ||
				valueType == typeof(ushort) || valueType == typeof(int) || valueType == typeof(uint))
				return EditorGUI.IntField(position, label, (int)value);
			if (valueType == typeof(long) || valueType == typeof(ulong))
				return EditorGUI.LongField(position, label, (long)value);
			if (valueType == typeof(float) || valueType == typeof(double) || valueType == typeof(decimal))
				return EditorGUI.FloatField(position, label, (float)value);
			if (valueType == typeof(string))
				return EditorGUI.TextField(position, label, (string)value);
			if (valueType == typeof(Color))
				return EditorGUI.ColorField(position, label, (Color)value);
			if (valueType == typeof(Color32))
				return EditorGUI.ColorField(position, label, (Color32)value);
			if (valueType == typeof(LayerMask))
				return EditorGUI.LayerField(position, label, (LayerMask)value);
			if (valueType.IsEnum)
				return EditorGUI.EnumPopup(position, label, (Enum)value);
			if (valueType == typeof(Vector2))
				return EditorGUI.Vector2Field(position, label, (Vector2)value);
			if (valueType == typeof(Vector3))
				return EditorGUI.Vector3Field(position, label, (Vector3)value);
			if (valueType == typeof(Vector4))
				return EditorGUI.Vector4Field(position, label, (Vector4)value);
			if (valueType == typeof(Rect))
				return EditorGUI.RectField(position, label, (Rect)value);
			if (valueType == typeof(AnimationCurve))
				return EditorGUI.CurveField(position, label, (AnimationCurve)value);
			if (valueType == typeof(Bounds))
				return EditorGUI.BoundsField(position, label, (Bounds)value);
			if (valueType == typeof(Quaternion))
				return Quaternion.Euler(EditorGUI.Vector3Field(position, label, ((Quaternion)value).eulerAngles));
			if (valueType == typeof(Vector2Int))
				return EditorGUI.Vector2IntField(position, label, (Vector2Int)value);
			if (valueType == typeof(Vector3Int))
				return EditorGUI.Vector3IntField(position, label, (Vector3Int)value);
			if (valueType == typeof(RectInt))
				return EditorGUI.RectIntField(position, label, (RectInt)value);
			if (valueType == typeof(BoundsInt))
				return EditorGUI.BoundsIntField(position, label, (BoundsInt)value);
			if (valueType == typeof(Gradient))
				return EditorGUI.GradientField(position, label, (Gradient)value);
			return value;
		}
		public static object ValueFieldLayout(string label, object value, Type valueType, bool includeChildren = true) {
			if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
				return EditorGUILayout.ObjectField(label, value as UnityEngine.Object, valueType, includeChildren);
			if (valueType == typeof(byte) || valueType == typeof(sbyte) || valueType == typeof(short) ||
				valueType == typeof(ushort) || valueType == typeof(int) || valueType == typeof(uint))
				return EditorGUILayout.IntField(label, (int)value);
			if (valueType == typeof(long) || valueType == typeof(ulong))
				return EditorGUILayout.LongField(label, (long)value);
			if (valueType == typeof(float) || valueType == typeof(double) || valueType == typeof(decimal))
				return EditorGUILayout.FloatField(label, (float)value);
			if (valueType == typeof(string))
				return EditorGUILayout.TextField(label, (string)value);
			if (valueType == typeof(Color) || valueType == typeof(Color32))
				return EditorGUILayout.ColorField(label, (Color)value);
			if (valueType == typeof(LayerMask))
				return EditorGUILayout.LayerField(label, (LayerMask)value);
			if (valueType.IsEnum)
				return EditorGUILayout.EnumPopup(label, (Enum)value);
			if (valueType == typeof(Vector2))
				return EditorGUILayout.Vector2Field(label, (Vector2)value);
			if (valueType == typeof(Vector3))
				return EditorGUILayout.Vector3Field(label, (Vector3)value);
			if (valueType == typeof(Vector4))
				return EditorGUILayout.Vector4Field(label, (Vector4)value);
			if (valueType == typeof(Rect))
				return EditorGUILayout.RectField(label, (Rect)value);
			if (valueType == typeof(AnimationCurve))
				return EditorGUILayout.CurveField(label, (AnimationCurve)value);
			if (valueType == typeof(Bounds))
				return EditorGUILayout.BoundsField(label, (Bounds)value);
			if (valueType == typeof(Quaternion))
				return Quaternion.Euler(EditorGUILayout.Vector3Field(label, ((Quaternion)value).eulerAngles));
			if (valueType == typeof(Vector2Int))
				return EditorGUILayout.Vector2IntField(label, (Vector2Int)value);
			if (valueType == typeof(Vector3Int))
				return EditorGUILayout.Vector3IntField(label, (Vector3Int)value);
			if (valueType == typeof(RectInt))
				return EditorGUILayout.RectIntField(label, (RectInt)value);
			if (valueType == typeof(BoundsInt))
				return EditorGUILayout.BoundsIntField(label, (BoundsInt)value);
			return value;
		}

		public static void HorizontalLine(Rect position, float height, Color color) {
			Rect rect = EditorGUI.IndentedRect(position);
			rect.y += (EditorGUIUtility.singleLineHeight - height) / 2;
			rect.height = height;
			EditorGUI.DrawRect(rect, color);
		}


		private static bool IsPropertyDrawerOf(this Type drawer, Type targetType) {
			return drawer.GetCustomAttributes<CustomPropertyDrawer>().Any(attribute => {
				var drawerTargetType = attribute.Reflect().GetGettableValues<Type>("m_Type").First().Get();
				// TODO 使用该 CustomPropertyDrawer 的 useForChildren 字段判断是否绘制派生类
				return drawerTargetType == targetType;
			});
		}

		private static Dictionary<Type, PropertyDrawer> _drawerDict = new Dictionary<Type, PropertyDrawer>();
		private static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer, bool setFieldInfo, FieldInfo fieldInfo, bool setAttribute, Attribute attribute) {
			drawer = _drawerDict.GetOrInit(
				targetType,
				() => {
					if (TypeCache.GetTypesDerivedFrom<PropertyDrawer>().TryGetFirst(out var drawerType, type => type.IsPropertyDrawerOf(targetType))) {
						return (PropertyDrawer)drawerType.InvokeMember(
							null,
							BindingFlags.DeclaredOnly |
							BindingFlags.Public | BindingFlags.NonPublic |
							BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null
						);
					}
					return null;
				}
			);
			if (drawer != null) {
				if (setFieldInfo) drawer.Reflect().GetSettableValues<FieldInfo>("m_FieldInfo").First().Set(fieldInfo);
				if (setAttribute) drawer.Reflect().GetSettableValues<Attribute>("m_Attribute").First().Set(attribute);
				return true;
			}
			return false;
		}
		public static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer)
			=> TryGetDrawer(targetType, out drawer, false, null, false, null);
		public static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer, FieldInfo fieldInfo)
			=> TryGetDrawer(targetType, out drawer, true, fieldInfo, false, null);
		public static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer, Attribute attribute)
			=> TryGetDrawer(targetType, out drawer, false, null, true, attribute);
		public static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer, FieldInfo fieldInfo, Attribute attribute)
			=> TryGetDrawer(targetType, out drawer, true, fieldInfo, true, attribute);



		public const float IndentLength = 15.0f;
		public const float HorizontalSpacing = 2.0f;

		private static GUIStyle _buttonStyle = new GUIStyle(GUI.skin.button) { richText = true };

		public static float GetIndentLength(Rect sourceRect) {
			Rect indentRect = EditorGUI.IndentedRect(sourceRect);
			float indentLength = indentRect.x - sourceRect.x;

			return indentLength;
		}

		public static void BeginBoxGroup_Layout(string label = "") {
			EditorGUILayout.BeginVertical(GUI.skin.box);
			if (!string.IsNullOrEmpty(label)) {
				EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
			}
		}

		public static void EndBoxGroup_Layout() {
			EditorGUILayout.EndVertical();
		}
	}
}
