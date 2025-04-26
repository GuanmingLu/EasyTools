using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace EasyTools {

	[Serializable]
	public class StaticReferences : ISerializationCallbackReceiver {
		private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		[SerializeField] private string m_json;
		[SerializeField] private SerializableDictionary<string, UObj> m_objDict;
		private Dictionary<FieldInfo, object> _dict = new();
		public Dictionary<FieldInfo, object> Dict => _dict;
		private string GetKey(FieldInfo field) => $"{field.DeclaringType.AssemblyQualifiedName} -> {field.Name}";
		private FieldInfo ParseKey(string key) {
			var pair = key.Split(" -> ");
			return Type.GetType(pair[0]).GetField(pair[1], flags);
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize() {
			// json -> dict
			try {
				var data = JObject.Parse(m_json);
				foreach (var (key, value) in data) {
					try {
						var field = ParseKey(key);
						if (field != null) Dict[field] = value.ToObject(field.FieldType);
					}
					catch { }
				}
			}
			catch { }
			// objDict -> dict
			foreach (var (key, obj) in m_objDict as Dictionary<string, UObj>) {
				try {
					var field = ParseKey(key);
					if (field != null) Dict[field] = obj;
				}
				catch { }
			}
			// fill dict if not exists
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
				var asmName = asm.FullName;
				if (
					asmName.StartsWith("Unity.") || asmName.StartsWith("UnityEngine.") || asmName.StartsWith("UnityEditor.") ||
					asmName.StartsWith("System.") || asmName.StartsWith("mscorlib")
				) continue;
				foreach (var type in asm.GetTypes()) {
					foreach (var field in type.GetFields(flags)) {
						if (field.GetCustomAttribute<SerializeField>() != null) Dict.TryAdd(field, field.GetValue(null));
					}
				}
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize() {
			JObject data = new();
			m_objDict.Clear();
			foreach (var (field, obj) in Dict) {
				var key = GetKey(field);
				if (typeof(UObj).IsAssignableFrom(field.FieldType)) m_objDict[key] = obj as UObj;
				else data[key] = obj == null ? null : JToken.FromObject(obj, JsonSerializer.Create(JsonExtensions.DefaultSettings));
			}
			m_json = data.ToString(Formatting.None);
		}

		public void SetAllValues() {
			foreach (var (field, value) in Dict) {
				if (field == null || value == null) continue;
				field.SetValue(null, value);
			}
		}
	}
}
