using System.Collections;
using EasyTools.InternalComponent;
using UnityEngine;

namespace EasyTools {
	/// <summary>
	/// 可以淡入淡出的遮罩
	/// </summary>
	public static class TransMask {
		private static TransitionMaskComponent Instance => TransitionMaskComponent.Instance;

		private static void SetColor(uint color) {
			Color c = ColorExt.Parse(color);
			c.a = Instance._mask.color.a;
			Instance._mask.color = c;
		}
		private static void SetAlpha(float alpha) => Instance._mask.SetA(alpha);


		public static string Text {
			get => Instance._text.text;
			set => Instance._text.text = value;
		}

		/// <summary>
		/// 停止淡入与淡出
		/// </summary>
		public static void StopFade() => Instance.StopAllCoroutines();

		/// <summary>
		/// 启用或禁用遮罩
		/// </summary>
		public static void EnableMask(bool enable) => Instance._mask.enabled = enable;

		/// <summary>
		/// 遮罩淡入
		/// </summary>
		public static CoroutineHandler ShowMask(float fadeSeconds = 0.5f, uint color = 0x000000FF) {
			SetColor(color);
			EnableMask(true);
			return EasyTween.Linear(fadeSeconds, SetAlpha).Run(Instance);
		}

		/// <summary>
		/// 遮罩淡出
		/// </summary>
		public static CoroutineHandler HideMask(float fadeSeconds = 0.5f, uint color = 0x000000FF) {
			SetColor(color);
			return EasyTween.Linear(fadeSeconds, d => SetAlpha(1 - d)).Then(() => EnableMask(false)).Run(Instance);
		}

		public static void LoadScene(string sceneName, float fadeTime = 0.5f) {
			StopFade();
			LoadSceneCoroutine(sceneName, fadeTime).Run(Instance);
		}

		public static IEnumerator LoadSceneCoroutine(string sceneName, float fadeTime = 0.5f) {
			yield return ShowMask(fadeTime);
			yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
			yield return HideMask(fadeTime);
		}
	}
}
