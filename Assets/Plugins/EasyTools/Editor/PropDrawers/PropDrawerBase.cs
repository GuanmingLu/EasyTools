using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {

	public abstract class PropDrawerBase : PropertyDrawer {
		public abstract float GetPropertyHeight(object propValue, string label, Object owner = null);
		public abstract void OnGUI(Rect position, object propValue, string label, Object owner = null);
		public abstract float GetPropertyHeight(object propValue, GUIContent label, Object owner = null);
		public abstract void OnGUI(Rect position, object propValue, GUIContent label, Object owner = null);
	}

	public abstract class PropDrawerBase<T> : PropDrawerBase {
		protected GUIContent label { get; set; }
		protected Object owner { get; set; }
		protected T propValue { get; private set; }
		protected SerializedProperty property { get; private set; }
		protected bool useProp => property != null;

		protected virtual float PropertyHeight => Utils.SingleLinePropHeight;
		public override float GetPropertyHeight(object propValue, GUIContent label, Object owner = null) {
			this.owner = owner;
			this.property = null;
			this.propValue = (T)propValue;
			this.label = label;
			return PropertyHeight;
		}
		public override float GetPropertyHeight(object propValue, string label, Object owner = null)
			=> GetPropertyHeight(propValue, new GUIContent(label), owner);
		public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			this.owner = property.serializedObject.targetObject;
			this.property = property;
			this.propValue = (T)fieldInfo.GetValue(owner);
			this.label = label;
			return PropertyHeight;
		}

		protected abstract void OnGUI(Rect position);
		public override void OnGUI(Rect position, object propValue, GUIContent label, Object owner = null) {
			this.owner = owner;
			this.property = null;
			this.propValue = (T)propValue;
			this.label = label;
			OnGUI(position);
		}
		public override void OnGUI(Rect position, object propValue, string label, Object owner = null)
			=> OnGUI(position, propValue, new GUIContent(label), owner);
		public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			this.owner = property.serializedObject.targetObject;
			this.property = property;
			this.propValue = (T)fieldInfo.GetValue(owner);
			this.label = label;
			OnGUI(position);
		}
	}
}
