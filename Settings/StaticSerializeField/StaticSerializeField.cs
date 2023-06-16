using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using EasyTools.Reflection;
#if UNITY_EDITOR
using EasyTools.Inspector;
using UnityEditor;
#endif

namespace EasyTools.Settings {

	public class StaticSerializeField : ResourceSingleton<StaticSerializeField> {
		public static StaticSerializeField GetInstance() => Instance;

		#region DataRegion

		#endregion DataRegion

		private static bool TryGetClassFromName(string s, out Type type) {
			try {
				type = Type.GetType(System.Text.Encoding.UTF8.GetString(EasyConvert.FromBase62(s[1..])));
				return true;
			}
			catch {
				type = null;
				return false;
			}
		}
		private static string ToClassName(Type type) => "_" + EasyConvert.ToBase62(System.Text.Encoding.UTF8.GetBytes(type.AssemblyQualifiedName));
		private static string GetDefinitionName(Type type) {
			var name = type.FullName;
			if (type.IsGenericType) {
				var index = name.IndexOf('`');
				if (index > 0) name = name[..index];
				name += "<";
				var args = type.GetGenericArguments();
				for (int i = 0; i < args.Length; i++) {
					if (i > 0) name += ",";
					name += GetDefinitionName(args[i]);
				}
				name += ">";
			}
			return name;
		}

#if UNITY_EDITOR

		[SerializeField] private InspectorButton m_refreshButton = new InspectorButton(nameof(Refresh), "刷新");

		private void Refresh() {
			var filePath = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this))[6..^0];

			var texts = System.IO.File.ReadAllLines(filePath).ToList();

			var start = texts.FindIndex(s => s.Contains("#region DataRegion"));
			var end = texts.FindIndex(s => s.Contains("#endregion DataRegion"));
			if (start < 0 || end < 0 || end - start < 1) Debug.LogError($"无法找到标记位置，start={start}, end={end}");
			else {
				texts.RemoveRange(start + 1, end - start - 1);

				static IEnumerable<string> GetDefines() {
					yield return string.Empty;
					foreach (var v in TypeCache.GetFieldsWithAttribute<SerializeField>().Where(f => f.IsStatic).GroupBy(f => f.DeclaringType)) {
						var className = ToClassName(v.Key);
						yield return "\t\t[Serializable]";
						yield return $"\t\tprivate class {className} {{";
						foreach (var f in v) {
							yield return $"\t\t\t[SerializeField] private {GetDefinitionName(f.FieldType)} {f.Name};";
						}
						yield return "\t\t}";
						yield return $"\t\t[SerializeField] private {className} {v.Key.Name};";
						yield return string.Empty;
					}
				}
				texts.InsertRange(start + 1, GetDefines());

				System.IO.File.WriteAllLines(filePath, texts);
				AssetDatabase.Refresh();
			}
		}

#endif

		private const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			foreach (var typeDataField in
				typeof(StaticSerializeField).GetFields(flags).Where(f => f.FieldType.IsClass && f.FieldType.Name[0] == '_' && f.FieldType.IsSerializable)
			) {
				if (TryGetClassFromName(typeDataField.FieldType.Name, out var targetType)) {
					foreach (var valueDataField in typeDataField.FieldType.GetFields(flags)) {
						var v = valueDataField.GetValue(typeDataField.GetValue(Instance));
						Debug.Log($"{targetType} -> {valueDataField.Name} = {v}");
					}
				}
			}
		}

	}
}
