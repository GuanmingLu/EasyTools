using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyTools {

	/// <summary>
	/// 一个简单的多语言、外部文件读取支持类
	/// </summary>
	public static class EasyLocalization {
		public const string DefaultLang = "zh-CN";
		public static string CurrentLang { get; private set; } = DefaultLang;

		private static Dictionary<string, JToken> _lib;
		private static bool Reload() {
			var path = $"EasyTools/Localization/{CurrentLang}";
			var pathLen = path.Length + 1;
			if (!StreamingAssetsReader.IsDirectory(path)) return false;
			if (_lib == null) _lib = new();
			else _lib.Clear();
			foreach (var filePath in StreamingAssetsReader.EnumerateFiles(path, true)) {
				try {
					_lib[Path.GetFileNameWithoutExtension(filePath)] = JToken.Parse(StreamingAssetsReader.ReadAllText(filePath));
				}
				catch { }
			}
			return true;
		}
		public static Dictionary<string, JToken> Lib {
			get {
				if (_lib == null) Reload();
				return _lib;
			}
		}

		public static event Action OnLangSwitched;

		[RuntimeInitializeOnLoadMethod]
		private static void OnStartup() {
			// TODO 读取设置并设置语言
			SwitchLang(DefaultLang);
		}

		/// <summary>
		/// 切换到指定的语言
		/// </summary>
		public static bool SwitchLang(string lang) {
			var oldLang = CurrentLang;
			CurrentLang = lang;
			if (Reload()) {
				OnLangSwitched?.Invoke();
				return true;
			}
			else {
				CurrentLang = oldLang;
				return false;
			}
		}

		/// <summary>
		/// 从指定的翻译文件中获取指定键或索引所对应的值
		/// </summary>
		public static T Get<T>(string fileName, object keyOrIndex) => Lib[fileName][keyOrIndex].ToObject<T>();

		/// <summary>
		/// 从指定的翻译文件中获取指定键或索引所对应的值
		/// </summary>
		/// <returns> 是否获取到 </returns>
		public static bool TryGet<T>(string fileName, object keyOrIndex, out T value) {
			value = default;
			return Lib.GetValueOrDefault(fileName)?[keyOrIndex]?.TryToObj(out value) ?? false;
		}

		/// <summary>
		/// 从指定的翻译文件中获取指定索引所对应的值
		/// </summary>
		/// <returns> 是否获取到 </returns>
		public static bool TryGet<T>(string fileName, int index, out T value) {
			value = default;
			return Lib.GetValueOrDefault(fileName) is JArray arr && index < arr.Count && arr[index].TryToObj(out value);
		}


		public static IEnumerable<T> AsArray<T>(string fileName) {
			var token = Lib.GetValueOrDefault(fileName);
			if (token.Type == JTokenType.Array)
				foreach (var item in token) {
					if (item.TryToObj(out T value)) yield return value;
				}
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("EasyTools/Localization/Reset Default Lang")]
		public static void Editor_ResetDefaultLang() => SwitchLang(DefaultLang);
#endif
	}
}
