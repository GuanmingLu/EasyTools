using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EasyTools {

	public static class EasyVariable {
		private static Dictionary<string, object> dict;

		public static bool Reload() {
			var path = "EasyTools/Variables.json";
			if (!StreamingAssetsReader.IsFile(path)) {
				Debug.LogError("EasyTools/Variables.json文件不存在");
				return false;
			}
			var json = StreamingAssetsReader.ReadAllText(path);
			dict = json.FromJson<Dictionary<string, object>>();
			return true;
		}

		public static bool Contains(string key) {
			if (dict == null && !Reload()) return false;
			else return dict.ContainsKey(key);
		}

		public static T Get<T>(string key) {
			if (dict == null && !Reload()) throw new System.Exception("EasyVariable.Reload()失败");
			else if (dict.TryGetValue(key, out var data)) return data.JsonConvertTo<T>();
			else throw new System.Exception($"EasyVariable中没有找到 {key} 的值");
		}

		public static bool TryGet<T>(string key, out T value) {
			if (dict == null && !Reload()) {
				value = default;
				return false;
			}
			else if (dict.TryGetValue(key, out var data)) {
				value = data.JsonConvertTo<T>();
				return true;
			}
			else {
				value = default;
				return false;
			}
		}
	}
}
