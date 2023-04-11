using System;
using Newtonsoft.Json;

namespace EasyTools {
	public static class JsonExtensions {
		public static string ToJson(this object obj)
			=> JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
		public static object FromJson(this string json) => JsonConvert.DeserializeObject(json);
		public static object FromJson(this string json, Type type) => JsonConvert.DeserializeObject(json, type);
		public static T FromJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json);
		public static T JsonConvertTo<T>(this object obj) => FromJson<T>(ToJson(obj));
	}
}
