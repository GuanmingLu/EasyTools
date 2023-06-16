using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {

	public abstract class PropDrawerBase<T> : PropertyDrawer {
		protected SerializedProperty property;
		protected GUIContent label;
		public Object Target => property.serializedObject.targetObject;
		protected T PropValue => (T)fieldInfo.GetValue(Target);

		public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			this.property = property;
			this.label = label;
			return PropertyHeight;
		}
		protected virtual float PropertyHeight => Utils.SingleLinePropHeight;

		public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			this.property = property;
			this.label = label;
			OnGUI(position);
		}
		protected virtual void OnGUI(Rect position) {
			EditorGUI.PropertyField(position, property, label);
		}
	}
}
