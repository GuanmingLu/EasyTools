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
		public static void Show(string message, float duration = 3f, uint color = ColorU.white) => Instance.ShowMsg(message, duration, color);

		public static void Log(string message, float duration = 3f, bool forceShow = false) {
			if (forceShow || EasyToolsSettings.ShowScrollLog) {
				Debug.Log(message);
				Instance.ShowMsg($"[Log] {message}", duration);
			}
		}

		public static void Warn(string message, float duration = 3f, bool forceShow = false) {
			if (forceShow || EasyToolsSettings.ShowScrollLog) {
				Debug.LogWarning(message);
				Instance.ShowMsg($"[Warn] {message}", duration, 0xFFC107FF);
			}
		}

		public static void Error(string message, float duration = 3f, bool forceShow = false) {
			if (forceShow || EasyToolsSettings.ShowScrollLog) {
				Debug.LogError(message);
				Instance.ShowMsg($"[Error] {message}", duration, 0xFF534AFF);
			}
		}
	}
}
