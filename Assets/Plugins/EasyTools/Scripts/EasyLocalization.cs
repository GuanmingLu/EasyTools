using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EasyTools {

	/// <summary>
	/// 一个简单的多语言、外部文件读取支持类
	/// </summary>
	public static class EasyLocalization {
		public const string DefaultLang = "zh-CN";
		public static string CurrentLang { get; set; } = DefaultLang;

		private static Dictionary<string, Dictionary<string, object>> _lib;

		public static event Action onLangSwitched;

		[RuntimeInitializeOnLoadMethod]
		private static void OnStartup() {
			// TODO 读取设置并设置语言
			SwitchLang(DefaultLang);
		}

		/// <summary>
		/// 切换到指定的语言
		/// </summary>
		public static bool SwitchLang(string lang) {
			if (SetLang(lang)) {
				onLangSwitched?.Invoke();
				return true;
			}
			else return false;
		}
		private static bool SetLang(string lang) {
			var path = Application.streamingAssetsPath + $"/EasyTools/Localization/{lang}";
			if (!Directory.Exists(path)) return false;
			CurrentLang = lang;
			var allFiles = Directory.GetFiles(path, "*.json");
			if (_lib == null) _lib = new Dictionary<string, Dictionary<string, object>>();
			else _lib.Clear();
			foreach (var fileName in allFiles) {
				_lib[Path.GetFileNameWithoutExtension(fileName)] = File.ReadAllText(fileName).FromJson<Dictionary<string, object>>();
			}
			return true;
		}
		public static bool ResetDefaultLang() => SetLang(DefaultLang);

		/// <summary>
		/// 从指定的翻译文件中获取指定键所对应的值（以指定的类型返回）
		/// </summary>
		public static T Get<T>(string fileName, string key) {
			if (_lib == null && !ResetDefaultLang()) return default;
			if (_lib.TryGetValue(fileName, out var dict) && dict.TryGetValue(key, out var value)) return value.JsonConvertTo<T>();
			else return default;
		}

		/// <summary>
		/// 从指定的翻译文件中获取指定键所对应的值，以指定的类型赋值给 value
		/// </summary>
		/// <returns> 是否获取到 </returns>
		public static bool TryGet<T>(string fileName, string key, out T value) {
			if (_lib == null && !ResetDefaultLang()) {
				value = default;
				return false;
			}
			if (_lib.TryGetValue(fileName, out var dict) && dict.TryGetValue(key, out var v)) {
				value = v.JsonConvertTo<T>();
				return true;
			}
			else {
				value = default;
				return false;
			}
		}


#if UNITY_EDITOR
		[UnityEditor.MenuItem("EasyTools/Localization/Reset Default Lang")]
		public static void EditorResetDefaultLang() => ResetDefaultLang();
#endif
	}
}
