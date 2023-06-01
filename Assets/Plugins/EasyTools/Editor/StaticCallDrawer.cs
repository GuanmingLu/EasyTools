using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EasyTools.InternalComponent;
using System;
using EasyTools.Reflection;
using System.Linq;
using System.Reflection;

namespace EasyTools.Editor {
	[CustomPropertyDrawer(typeof(StaticCall))]
	public class StaticCallDrawer : PropertyDrawer {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var className = property.FindPropertyRelative("className");
			var methodName = property.FindPropertyRelative("methodName");

			var type = Type.GetType(className.stringValue);
			var (classText, methodText) = type != null
				? type.GetReflector().GetMembers().Any(m => m.Name == methodName.stringValue && m is MethodInfo method && method.IsStatic)
					? (type.Name, methodName.stringValue)
					: (type.Name, $"<Missing> {methodName.stringValue}")
				: ($"<Missing> {className.stringValue}", methodName.stringValue);

			position.width /= 2;

			if (GUI.Button(position, classText)) {
				GenericMenu menu = new();
				var types = TypeCache.GetTypesDerivedFrom<object>();
				foreach (var t in types) {
					if (t.GetMethods().Any(m => m.IsStatic && m.IsCallable())) {
						var asmName = t.Assembly.GetName().Name;
						var root = asmName.StartsWith("Assembly-CSharp") ? asmName : $"Others";
						menu.AddItem(new GUIContent($"{root}/{t.FullName.Replace('.', '/')}"), false, type => {
							className.stringValue = type as string;
							className.serializedObject.ApplyModifiedProperties();
						}, t.AssemblyQualifiedName);
					}
				}
				menu.ShowAsContext();
			}

			position.x += position.width;

			if (type != null && GUI.Button(position, methodText)) {
				GenericMenu menu = new();
				var methodList = type.GetMethods().Where(m => m.IsStatic && m.IsCallable());
				foreach (var method in methodList) {
					menu.AddItem(new GUIContent(method.Name), false, method => {
						methodName.stringValue = method as string;
						methodName.serializedObject.ApplyModifiedProperties();
					}, method.Name);
				}
				menu.ShowAsContext();
			}
		}
	}

}
