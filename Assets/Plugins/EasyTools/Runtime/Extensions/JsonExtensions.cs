using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyTools {
	public static class JsonExtensions {
		private static JsonSerializerSettings _settings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
		public static string ToJson(this object obj)
			=> JsonConvert.SerializeObject(obj, _settings);
		public static object FromJson(this string json) => JsonConvert.DeserializeObject(json, _settings);
		public static object FromJson(this string json, Type type) => JsonConvert.DeserializeObject(json, type, _settings);
		public static T FromJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json, _settings);
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
	}
}
