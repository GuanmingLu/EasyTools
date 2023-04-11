using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using EasyTools.Inspector;

namespace EasyTools.Editor {

	[CustomPropertyDrawer(typeof(PropModifier), true)]
	public class PropModifiersDrawer : PropertyDrawer {
		private bool TryGetDrawer(out PropertyDrawer drawer) => Utils.TryGetDrawer(fieldInfo.FieldType, out drawer, fieldInfo);

		private IEnumerable<PropModifier> _modifiers;
		private IEnumerable<PropModifier> Modifiers {
			get {
				if (_modifiers == null) _modifiers = fieldInfo.GetCustomAttributes<PropModifier>(true).OrderBy(mod => mod.order);
				return _modifiers;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var height = TryGetDrawer(out var drawer) ? drawer.GetPropertyHeight(property, label) : EditorGUI.GetPropertyHeight(property.propertyType, label);
			var draw = Modifiers.All(mod => mod.ChangeHeight(ref height, ref property, ref label, fieldInfo));
			return draw ? height : -EditorGUIUtility.standardVerticalSpacing;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			int count = 0;
			var draw = Modifiers.All(mod => {
				count++;
				return mod.BeforeGUI(ref position, ref property, ref label, fieldInfo);
			});

			if (draw) {
				if (TryGetDrawer(out var drawer))
					drawer.OnGUI(position, property, label);
				else
					EditorGUI.PropertyField(position, property, label);
			}

			Modifiers.Take(count).Reverse().ForEach(mod => mod.AfterGUI());
		}
	}
}
