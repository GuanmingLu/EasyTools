using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTools.InternalComponent;
using UnityEngine.UI;

namespace EasyTools {
	partial class EasyToolsGameObject {
		[Header("TransMask")]
		[SerializeField] private Image m_mask;
		[SerializeField] private Text m_text;
		[SerializeField] private GameObject m_loading;

		private CanvasGroup _canvasGroup;
		private CanvasGroup CanvasGroup => _canvasGroup ??= m_mask.GetComponent<CanvasGroup>();

		private void InitTransMask() {
			MaskColor = Color.black;
			MaskAlpha = 0;
			MaskText = "";
			MaskEnabled = false;
		}

		internal bool MaskEnabled {
			get => m_mask.gameObject.activeSelf;
			set => m_mask.gameObject.SetActive(value);
		}

		internal float MaskAlpha {
			get => CanvasGroup.alpha;
			set => CanvasGroup.alpha = value;
		}

		internal Color MaskColor {
			get => m_mask.color;
			set => m_mask.color = value;
		}

		internal string MaskText {
			get => m_text.text;
			set => m_text.text = value;
		}

		internal bool IsLoading {
			get => m_loading != null && m_loading.activeSelf;
			set => m_loading?.SetActive(value);
		}
	}

	/// <summary>
	/// 可以淡入淡出的遮罩
	/// </summary>
	public static class TransMask {
		private static EasyToolsGameObject Instance => EasyToolsGameObject.Instance;

		/// <summary>
		/// 是否启用遮罩
		/// </summary>
		public static bool MaskEnabled {
			get => Instance.MaskEnabled;
			set => Instance.MaskEnabled = value;
		}

		/// <summary>
		/// 遮罩颜色
		/// </summary>
		public static uint MaskColor {
			get => Instance.MaskColor.ToUInt();
			set => Instance.MaskColor = ColorExt.Parse(value);
		}

		/// <summary>
		/// 遮罩上的文字
		/// </summary>
		public static string Text {
			get => Instance.MaskText;
			set => Instance.MaskText = value;
		}

		/// <summary>
		/// 是否显示加载图标
		/// </summary>
		public static bool IsLoading {
			get => Instance.IsLoading;
			set => Instance.IsLoading = value;
		}

		private static Easing _showMask = new Easing(Instance, 0, 1, EasingType.Linear)
			.OnStartForward(() => MaskEnabled = true)
			.OnFinishBackward(() => MaskEnabled = false)
			.OnValueChanged(t => Instance.MaskAlpha = t);

		/// <summary>
		/// 停止淡入与淡出
		/// </summary>
		public static void StopFade() => _showMask.Stop();

		/// <summary>
		/// 遮罩淡入
		/// </summary>
		public static Coroutine ShowMask(float fadeSeconds = 0.5f, uint color = 0xFFFFFFFF) {
			MaskColor = color;
			return _showMask.SetDuration(fadeSeconds).RunForward().Coroutine;
		}

		/// <summary>
		/// 遮罩淡出
		/// </summary>
		public static Coroutine HideMask(float fadeSeconds = 0.5f, uint color = 0xFFFFFFFF) {
			MaskColor = color;
			return _showMask.SetDuration(fadeSeconds).RunBackward().Coroutine;
		}

		/// <summary>
		/// 简易的加载场景方法
		/// </summary>
		/// <remarks>
		/// 复杂的加载控制方法请参考 <see cref="GameOverExample"/>
		/// </remarks>
		public static Coroutine LoadScene(string sceneName) {
			IEnumerator C() {
				IsLoading = true;
				yield return ShowMask(0.5f);
				yield return SceneManager.LoadSceneAsync(sceneName);
				yield return HideMask(0.5f);
				IsLoading = false;
			}
			return C().RunOn(Instance);
		}

		/// <summary>
		/// 简易的加载场景方法
		/// </summary>
		/// <remarks>
		/// 复杂的加载控制方法请参考 <see cref="GameOverExample"/>
		/// </remarks>
		public static Coroutine LoadScene(int buildIndex) {
			IEnumerator C() {
				IsLoading = true;
				yield return ShowMask(0.5f);
				yield return SceneManager.LoadSceneAsync(buildIndex);
				yield return HideMask(0.5f);
				IsLoading = false;
			}
			return C().RunOn(Instance);
		}

		public static Coroutine ReloadScene() => LoadScene(SceneManager.GetActiveScene().buildIndex);

		public static Coroutine LoadNextScene() => LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

		/// <summary>
		/// 复杂控制示例
		/// </summary>
		public static void GameOverExample() {
			static IEnumerator C() {
				TransMask.Text = "游戏结束";
				TransMask.IsLoading = false;

				yield return TransMask.ShowMask(1);
				yield return Wait.Seconds(2);

				TransMask.IsLoading = true;

				yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
				yield return TransMask.HideMask(1);

				TransMask.IsLoading = false;
				TransMask.Text = "";
			}
			C().Run();
		}
	}
}
