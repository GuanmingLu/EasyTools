using UnityEngine;
using EasyTools.InternalComponent;
using EasyTools.Settings;

namespace EasyTools {
	/// <summary>
	/// 游戏内的滚动消息
	/// </summary>
	public static class ScrollMsg {
		private static EasyToolsGameObject Instance => EasyToolsGameObject.Instance;

		/// <summary>
		/// 添加一条滚动消息
		/// </summary>
		/// <param name="duration">该消息停留的时间</param>
		public static void Show(string message, float duration = 3f) => Instance.ShowMsg(message, duration);

		public static void Log(string message, float duration = 3f, bool forceShow = false) {
			if (forceShow || EasyToolsSettings.ShowScrollLog) {
				Debug.Log(message);
				Instance.ShowMsg($"[Log] {message}", duration);
			}
		}

		public static void Warn(string message, float duration = 3f, bool forceShow = false) {
			if (forceShow || EasyToolsSettings.ShowScrollLog) {
				Debug.LogWarning(message);
				Instance.ShowMsg($"<color=#ffc107>[Warn] {message}</color>", duration);
			}
		}

		public static void Error(string message, float duration = 3f, bool forceShow = false) {
			if (forceShow || EasyToolsSettings.ShowScrollLog) {
				Debug.LogError(message);
				Instance.ShowMsg($"<color=#ff534a>[Error] {message}</color>", duration);
			}
		}
	}
}
