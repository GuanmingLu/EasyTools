using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyTools {
	/// <summary>
	/// 可以淡入淡出的遮罩
	/// </summary>
	public static class TransMask {
		private static InternalComponent.TransitionMaskComponent Instance => EasyToolsPrefab.TransitionMaskComponent;

		public static bool MaskEnabled => Instance._mask.enabled;

		private static void SetColor(uint color) {
			Color c = ColorExt.Parse(color);
			c.a = Instance._mask.color.a;
			Instance._mask.color = c;
		}

		public static string Text {
			get => Instance._text.text;
			set => Instance._text.text = value;
		}

		/// <summary>
		/// 启用或禁用遮罩
		/// </summary>
		public static void EnableMask(bool enable) => Instance._mask.enabled = enable;

		static Coroutine _fadeCoroutine;

		/// <summary>
		/// 停止淡入与淡出
		/// </summary>
		public static void StopFade() {
			Instance.StopCoroutine(ref _fadeCoroutine);
		}

		/// <summary>
		/// 遮罩淡入
		/// </summary>
		public static Coroutine ShowMask(float fadeSeconds = 0.5f, uint color = 0x000000FF) {
			SetColor(color);
			EnableMask(true);
			EasyTween.Linear(fadeSeconds, Instance._mask.SetA, Instance._mask.color.a, 1).RunOn(Instance, ref _fadeCoroutine);
			return _fadeCoroutine;
		}

		/// <summary>
		/// 遮罩淡出
		/// </summary>
		public static Coroutine HideMask(float fadeSeconds = 0.5f, uint color = 0x000000FF) {
			SetColor(color);
			EasyTween.Linear(fadeSeconds, Instance._mask.SetA, Instance._mask.color.a, 0)
			.Then(() => EnableMask(false)).RunOn(Instance, ref _fadeCoroutine);
			return _fadeCoroutine;
		}

		public static Coroutine LoadScene(string sceneName, float fadeTime = 0.5f, uint color = 0x000000FF, GameObject loading = null) {
			IEnumerator C() {
				yield return ShowMask(fadeTime, color);
				loading?.SetActive(true);
				yield return SceneManager.LoadSceneAsync(sceneName);
				loading?.SetActive(false);
				yield return HideMask(fadeTime, color);
			}
			C().RunOn(Instance, ref _fadeCoroutine);
			return _fadeCoroutine;
		}

		public static Coroutine LoadScene(int buildIndex, float fadeTime = 0.5f, uint color = 0x000000FF, GameObject loading = null) {
			IEnumerator C() {
				yield return ShowMask(fadeTime, color);
				loading?.SetActive(true);
				yield return SceneManager.LoadSceneAsync(buildIndex);
				loading?.SetActive(false);
				yield return HideMask(fadeTime, color);
			}
			C().RunOn(Instance, ref _fadeCoroutine);
			return _fadeCoroutine;
		}

		public static Coroutine ReloadScene(float fadeTime = 0.5f, uint color = 0x000000FF, GameObject loading = null)
			=> LoadScene(SceneManager.GetActiveScene().buildIndex, fadeTime, color, loading);
	}
}
