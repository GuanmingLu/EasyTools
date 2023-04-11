using UnityEngine;
using UnityEditor;
using EasyTools.Editor;

namespace EasyTools.Editor {

	[CustomPropertyDrawer(typeof(IDrawableObject), true)]
	public class DrawableObjectDrawer : PropDrawerBase<IDrawableObject> {
		static float SingleHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		protected override float PropertyHeight {
			get {
				if (property.isExpanded) {
					var height = SingleHeight;
					foreach (var v in PropValue.GetObj()) {
						height += Utils.GetPropertyHeight(v.type, label) + EditorGUIUtility.standardVerticalSpacing;
					}
					return height;
				}
				else return SingleHeight;
			}
		}

		protected override void OnGUI(Rect position) {
			EditorGUI.BeginProperty(position, label, property);

			position.height = EditorGUIUtility.singleLineHeight;

			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);

			if (property.isExpanded) {  // 绘制内容

				position.y += SingleHeight;
				EditorGUI.indentLevel++;

				foreach (var (key, type, value) in PropValue.GetObj()) {
					position.height = Utils.GetPropertyHeight(type, label);
					if (PropValue is IEditableObject editable) {
						EditorGUI.BeginChangeCheck();
						var newValue = Utils.ValueField(position, key, value, type);
						if (EditorGUI.EndChangeCheck()) {
							Undo.RecordObject(property.serializedObject.targetObject, "Edit " + key);
							editable.SetObj(key, newValue);
							// EditorUtility.SetDirty(property.serializedObject.targetObject);
							break;
						}
					}
					else {
						using (new EditorGUI.DisabledScope(true)) Utils.ValueField(position, key, value, type);

					}
					position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				}

				EditorGUI.indentLevel--;
			}

			EditorGUI.EndProperty();
		}
	}
}
