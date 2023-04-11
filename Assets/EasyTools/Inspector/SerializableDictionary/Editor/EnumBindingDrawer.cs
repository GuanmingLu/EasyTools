using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace EasyTools.Editor {

	[CustomPropertyDrawer(typeof(EnumBinding<,>), true)]
	public class EnumBindingDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => Draw(position, property, label);

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => GetHeight(property);

		protected const string KeysFieldName = "m_keys";
		protected const string ValuesFieldName = "m_values";

		public virtual SerializedProperty GetKeyArrayProperty(SerializedProperty property) => property.FindPropertyRelative(KeysFieldName);
		public virtual SerializedProperty GetValueAt(SerializedProperty property, int i) {
			var prop = property.FindPropertyRelative(ValuesFieldName).GetArrayElementAtIndex(i);
			var inner = prop.FindPropertyRelative(ValuesFieldName);
			return inner != null ? inner : prop;
		}

		static float SingleHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		public float GetHeight(SerializedProperty property) {
			if (property.isExpanded) {
				var count = GetKeyArrayProperty(property).arraySize;
				var height = SingleHeight;
				for (int i = 0; i < count; i++) {
					var list = GetValueAt(property, i);
					height += EditorGUI.GetPropertyHeight(list) + EditorGUIUtility.standardVerticalSpacing;
				}
				return height;
			}
			else return SingleHeight;
		}

		public void Draw(Rect position, SerializedProperty property, GUIContent label, Color titleColor, Color labelColor) {
			Color defaultTitleColor = EditorStyles.foldout.normal.textColor;
			Color defaultLabelColor = EditorStyles.label.normal.textColor;
			EditorStyles.foldout.normal.textColor = titleColor;
			EditorStyles.label.normal.textColor = labelColor;

			EditorGUI.BeginProperty(position, label, property);

			position.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField(position, property, label, false);

			EditorStyles.foldout.normal.textColor = defaultTitleColor;

			if (property.isExpanded) {  // 绘制列表内容

				position.y += SingleHeight;
				EditorGUI.indentLevel++;

				var keyArrayProperty = GetKeyArrayProperty(property);

				for (int i = 0; i < keyArrayProperty.arraySize; i++) {
					var nameProperty = keyArrayProperty.GetArrayElementAtIndex(0).enumDisplayNames[i];
					var valueProperty = GetValueAt(property, i);

					position.height = EditorGUI.GetPropertyHeight(valueProperty);

					EditorGUI.PropertyField(
						position, valueProperty, new GUIContent(nameProperty), true
					);
					position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				}

				EditorGUI.indentLevel--;
			}

			EditorGUI.EndProperty();

			EditorStyles.label.normal.textColor = defaultLabelColor;
		}

		public void Draw(Rect position, SerializedProperty property, GUIContent label) {
			Draw(position, property, label, EditorStyles.foldout.normal.textColor, EditorStyles.label.normal.textColor);
		}
	}
}
