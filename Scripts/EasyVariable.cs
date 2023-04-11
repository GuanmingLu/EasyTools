using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EasyTools {

	public static class EasyVariable {
		private static Dictionary<string, object> dict;

		public static bool Reload() {
			var path = Application.streamingAssetsPath + $"/EasyTools/Variables.json";
			if (!File.Exists(path)) {
				Debug.LogError("EasyTools/Variables.json文件不存在");
				return false;
			}
			// TODO 异步读取
			var json = File.ReadAllText(path);
			dict = json.FromJson<Dictionary<string, object>>();
			return true;
		}

		public static T Get<T>(string key) {
			if (dict == null && !Reload()) return default;
			else if (dict.TryGetValue(key, out var data)) return data.JsonConvertTo<T>();
			else return default;
		}
	}
}
