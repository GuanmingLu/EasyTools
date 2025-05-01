using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyTools {
	public static class JsonExtensions {
		public static readonly JsonSerializerSettings DefaultSettings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

		public static string ToJson(this object obj)
			=> JsonConvert.SerializeObject(obj, DefaultSettings);
		public static object FromJson(this string json) => JsonConvert.DeserializeObject(json, DefaultSettings);
		public static object FromJson(this string json, Type type) => JsonConvert.DeserializeObject(json, type, DefaultSettings);
		public static T FromJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json, DefaultSettings);
		public static T JsonConvertTo<T>(this object obj) => FromJson<T>(ToJson(obj));
		public static object JsonConvertTo(this object obj, Type type) => FromJson(ToJson(obj), type);
		public static bool TryToObj<T>(this JToken self, out T value) {
			if (self != null) {
				value = self.ToObject<T>();
				return value != null || self.Type == JTokenType.Null;
			}
			value = default;
			return false;
		}

		public static IEnumerable<(string[] path, JProperty property)> EnumerateProperties(this JObject obj, bool recursive) {
			foreach (var prop in obj.Properties()) {
				var name = prop.Name;
				yield return (new[] { name }, prop);
				var value = prop.Value;
				if (recursive && value.Type == JTokenType.Object) {
					foreach (var (chPath, chProp) in value.Value<JObject>().EnumerateProperties(recursive)) {
						yield return (chPath.Insert(name, 0), chProp);
					}
				}
			}
		}
	}
}
