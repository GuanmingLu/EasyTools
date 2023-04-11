using UnityEngine;
using UnityEditor;
using EasyTools.Settings;
using System;
using System.Linq;

namespace EasyTools.Editor {
	public class UseAssetEditWindow : EditorWindow {
		[MenuItem("EasyTools/UseAsset")]
		public static void Open() {
			GetWindow<UseAssetEditWindow>();
		}

		private void OnEnable() {
			titleContent = new GUIContent("UseAsset");

			var settings = UseAssetSettings.GetInstance().useAssetFields;

			// 移除 Settings 中不存在或没有应用 UseAssetAttribute 的字段
			settings.RemoveAll(f => {
				var type = Type.GetType(f.typeAssemblyQualifiedName);
				if (type == null) return true;  // 不存在该类型，移除
				var field = type.GetField(f.fieldName, EasyTools.Settings.UseAssetSettings.BINDINGS);
				return field == null || !field.IsDefined(typeof(UseAssetAttribute), false); // 不存在该字段或该字段没有应用 UseAssetAttribute，移除
			});

			// 添加 Settings 中不存在的字段
			foreach (var field in TypeCache.GetFieldsWithAttribute<UseAssetAttribute>()) {
				if (!field.IsStatic) continue;
				var typeAssemblyQualifiedName = field.DeclaringType.AssemblyQualifiedName;
				var fieldName = field.Name;
				var fieldTypeAssemblyQualifiedName = field.FieldType.AssemblyQualifiedName;

				var isUObj = typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType);

				// 是否已存在匹配类名和字段名的字段
				var existField = settings.Find(f => f.typeAssemblyQualifiedName == typeAssemblyQualifiedName && f.fieldName == fieldName);
				if (existField is null) {
					// 不存在则添加
					settings.Add(new UseAssetSettings.UseAssetField {
						typeAssemblyQualifiedName = typeAssemblyQualifiedName,
						fieldName = fieldName,
						fieldTypeAssemblyQualifiedName = fieldTypeAssemblyQualifiedName,
						uObjValue = isUObj ? field.GetValue(null) as UnityEngine.Object : null,
						otherValueJson = isUObj ? null : field.GetValue(null).ToJson(),
					});
				}
				else {
					// 存在则更新其数据类型
					existField.fieldTypeAssemblyQualifiedName = fieldTypeAssemblyQualifiedName;
					if (isUObj) existField.otherValueJson = null;
					else existField.uObjValue = null;
				}
			}

			settings.Sort((a, b) => {
				var typeA = Type.GetType(a.typeAssemblyQualifiedName);
				var typeB = Type.GetType(b.typeAssemblyQualifiedName);
				if (typeA == typeB) return a.fieldName.CompareTo(b.fieldName);
				else return typeA.FullName.CompareTo(typeB.FullName);
			});

			// Debug.Log(UseAssetSettings.GetInstance().useAssetFields.ToJson());
		}

		private void OnGUI() {
			var settings = UseAssetSettings.GetInstance().useAssetFields.GroupBy(f => f.typeAssemblyQualifiedName);

			foreach (var group in settings) {
				var type = Type.GetType(group.Key);
				if (type == null) continue;

				EditorGUILayout.Foldout(true, type.FullName);

				EditorGUI.indentLevel++;

				foreach (var field in group) {
					var fieldType = Type.GetType(field.fieldTypeAssemblyQualifiedName);
					object value = typeof(UnityEngine.Object).IsAssignableFrom(fieldType) ? field.uObjValue : field.otherValueJson.FromJson(fieldType);

					EditorGUI.BeginChangeCheck();
					value = Utils.ValueFieldLayout(field.fieldName, value, fieldType);
					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(UseAssetSettings.GetInstance(), "Change UseAsset Field");
						if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType)) field.uObjValue = value as UnityEngine.Object;
						else field.otherValueJson = value.ToJson();
					}
				}

				EditorGUI.indentLevel--;
			}
		}
	}
}
