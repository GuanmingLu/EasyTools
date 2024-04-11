using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace EasyTools {

	public static class EasySave {
		private static Dictionary<string, object> data = new();

		private static string directory = "EasySave";
		private static string tempFileName = "EasySaveTemp";
		private static string IndexFileName(int index) => $"EasySave{index}";
		private static string SaveDir => Path.Combine(Application.persistentDataPath, directory);
		private static string GetFilePath(string fileName) => Path.Combine(Application.persistentDataPath, directory, fileName);

		public static bool DoClear { get; set; } = true;
		public static bool Exists() => Exists(tempFileName);
		public static bool Exists(int index) => Exists(IndexFileName(index));
		public static bool Exists(string fileName) => File.Exists(GetFilePath(fileName));

		public static void Save() => SaveTo(tempFileName);
		public static void SaveTo(int index) => SaveTo(IndexFileName(index));
		public static void SaveTo(string fileName) {
			Directory.CreateDirectory(SaveDir);
			if (DoClear) data.Clear();
			foreach (var (key, member) in members) {
				data[key] = member switch {
					FieldInfo field => field.GetValue(null),
					PropertyInfo prop => prop.GetValue(null),
					_ => null
				};
			}
			// TODO 异步保存
			File.WriteAllText(GetFilePath(fileName), data.ToJson());
		}

		public static void Load() => LoadFrom(tempFileName);
		public static void LoadFrom(int index) => LoadFrom(IndexFileName(index));
		public static void LoadFrom(string fileName) {
			if (DoClear) data.Clear();
			if (!Exists(fileName)) {
				Debug.LogError($"试图读取不存在的存档文件：{fileName}");
				return;
			}
			// TODO 异步读取
			var json = File.ReadAllText(GetFilePath(fileName));
			data = json.FromJson<Dictionary<string, object>>();
			foreach (var (key, member) in members) {
				if (data.TryGetValue(key, out var value)) {
					switch (member) {
						case FieldInfo field:
							field.SetValue(null, value);
							break;
						case PropertyInfo prop:
							prop.SetValue(null, value);
							break;
					}
				}
			}
		}

		public class RegAttribute : Attribute {
			public RegAttribute() { }
			public RegAttribute(string customKey) => Key = customKey;
			public string Key { get; }
		}

		private static Dictionary<string, MemberInfo> members = new();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RegisterAttribute() {
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
				foreach (var type in asm.GetTypes()) {
					foreach (var field in type.GetFields()) {
						var attr = field.GetCustomAttribute<RegAttribute>();
						if (attr == null) continue;
						if (!field.IsStatic) continue;
						var key = string.IsNullOrEmpty(attr.Key) ? $"{type.AssemblyQualifiedName} -> {field.Name}" : attr.Key;
						members.Add(key, field);
						Debug.Log($"EasySave reg field: {key}");
					}
					foreach (var prop in type.GetProperties()) {
						var attr = prop.GetCustomAttribute<RegAttribute>();
						if (attr == null) continue;
						if (!prop.CanWrite || !prop.CanRead) continue;
						if (!prop.GetSetMethod().IsStatic || !prop.GetGetMethod().IsStatic) continue;
						var key = string.IsNullOrEmpty(attr.Key) ? $"{type.AssemblyQualifiedName} -> {prop.Name}" : attr.Key;
						members.Add(key, prop);
						Debug.Log($"EasySave reg prop: {key}");
					}
				}
			}
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("EasyTools/EasySave/Open Directory")]
		private static void OpenDir() {
			Directory.CreateDirectory(SaveDir);
			System.Diagnostics.Process.Start(SaveDir);
		}

		[UnityEditor.MenuItem("EasyTools/EasySave/Clear Temp")]
		private static void ClearTemp() {
			File.Delete(GetFilePath(tempFileName));
		}
#endif
	}
}
