using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EasyTools.Reflection;
using System.Linq;

namespace EasyTools.Inspector {

	#region 抽象基类

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public abstract class PropModifier : PropertyAttribute {
#if UNITY_EDITOR
		public virtual bool ChangeHeight(ref float height, ref SerializedProperty property, ref GUIContent label, FieldInfo fieldInfo) => true;
		public virtual bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, FieldInfo fieldInfo) => true;
		public virtual void AfterGUI() { }
#endif
	}

	public abstract class AttributeWithValue : PropModifier {
		private string _valueName;
		protected AttributeWithValue(string valueName) => _valueName = valueName;
#if UNITY_EDITOR
		protected bool TryGetValue<TValue>(SerializedProperty property, out TValue result)
			=> property.serializedObject.targetObject.Reflect().TryGet(_valueName, out result);
#endif
	}

	#endregion

	/// <summary>
	/// 仅在给定条件（<see cref="bool"/> 值）满足时使该字段可编辑
	/// </summary>
	public class EnableIfAttribute : AttributeWithValue {
		public EnableIfAttribute(string nameof_condition) : base(nameof_condition) { }
#if UNITY_EDITOR
		private bool GetCondition(SerializedProperty property) => !TryGetValue<bool>(property, out var cd) || cd;
		public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, FieldInfo fieldInfo) {
			GUI.enabled = GetCondition(property);
			return true;
		}
		public override void AfterGUI() {
			GUI.enabled = true;
		}
#endif
	}

	/// <summary>
	/// 仅在给定条件（<see cref="bool"/> 值）满足时显示该字段
	/// </summary>
	public class ShowIfAttribute : AttributeWithValue {
		public ShowIfAttribute(string nameof_condition) : base(nameof_condition) { }
#if UNITY_EDITOR
		private bool GetCondition(SerializedProperty property) => !TryGetValue<bool>(property, out var cd) || cd;
		public override bool ChangeHeight(ref float height, ref SerializedProperty property, ref GUIContent label, FieldInfo fieldInfo) {
			if (GetCondition(property)) return true;
			height = 0;
			return false;
		}
		public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, FieldInfo fieldInfo) {
			if (GetCondition(property)) return true;
			position.height = 0;
			return false;
		}
#endif
	}

	/// <summary>
	/// 将字段添加到一个折叠组中
	/// </summary>
	public class FoldoutAttribute : PropModifier {
		public string FoldoutName { get; }
		public bool IsHeader { get; }
		/// <summary>
		/// 将字段添加到一个折叠组中
		/// </summary>
		/// <param name="foldoutName">折叠组的名称</param>
		/// <param name="isHeader">是否在该字段上方显示折叠组的标题</param>
		public FoldoutAttribute(string foldoutName, bool isHeader = false) {
			FoldoutName = foldoutName;
			IsHeader = isHeader;
		}
#if UNITY_EDITOR
		private string GetSavedKey(SerializedProperty property) => $"{property.serializedObject.targetObject.GetInstanceID()}:Foldout:{FoldoutName}";
		private bool GetIsExpanded(SerializedProperty property) => EditorPrefs.GetBool(GetSavedKey(property), true);
		private void SetIsExpanded(SerializedProperty property, bool value) => EditorPrefs.SetBool(GetSavedKey(property), value);
		public override bool ChangeHeight(ref float height, ref SerializedProperty property, ref GUIContent label, FieldInfo fieldInfo) {
			if (IsHeader) {
				if (GetIsExpanded(property)) height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				else height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				return true;
			}
			else {
				if (GetIsExpanded(property)) return true;
				height = 0;
				return false;
			}
		}
		public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, FieldInfo fieldInfo) {
			var isExpanded = GetIsExpanded(property);
			if (IsHeader) {
				var rect = new Rect(position);
				rect.height = EditorGUIUtility.singleLineHeight;
				var expanded = EditorGUI.Foldout(rect, isExpanded, new GUIContent(FoldoutName), true);
				if (expanded != isExpanded) {
					isExpanded = expanded;
					SetIsExpanded(property, isExpanded);
				}
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				position.height -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			EditorGUI.indentLevel++;
			if (isExpanded) return true;
			position.height = 0;
			return false;
		}
		public override void AfterGUI() {
			EditorGUI.indentLevel--;
		}
#endif
	}

	/// <summary>
	/// 将该序列化字段在 Inspector 中显示为只读
	/// </summary>
	public class ReadOnlyAttribute : PropModifier {
#if UNITY_EDITOR
		public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, FieldInfo fieldInfo) {
			GUI.enabled = false;
			return true;
		}
		public override void AfterGUI() {
			GUI.enabled = true;
		}
#endif
	}
}
