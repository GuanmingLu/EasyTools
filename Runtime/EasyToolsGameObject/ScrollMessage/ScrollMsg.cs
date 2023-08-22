using UnityEngine;
using EasyTools.InternalComponent;

namespace EasyTools {
	/// <summary>
	/// 游戏内的滚动消息
	/// </summary>
	public static class ScrollMsg {
		public static bool ShowLog { get; set; } = true;

		private static EasyToolsGameObject Instance => EasyToolsGameObject.Instance;

		/// <summary>
		/// 添加一条滚动消息
		/// </summary>
		/// <param name="duration">该消息停留的时间</param>
		public static void Show(string message, float duration = 3f) => Instance.ShowMsg(message, duration);

		public static void Log(string message, float duration = 3f) {
			if (ShowLog) {
				Debug.Log(message);
				Instance.ShowMsg("[Log] " + message, duration);
			}
		}

		public static void Warn(string message, float duration = 3f) {
			Debug.LogWarning(message);
			Instance.ShowMsg("[Warn] " + message, duration);
		}

		public static void Error(string message, float duration = 3f) {
			Debug.LogError(message);
			Instance.ShowMsg("[Error] " + message, duration);
		}
	}
}
