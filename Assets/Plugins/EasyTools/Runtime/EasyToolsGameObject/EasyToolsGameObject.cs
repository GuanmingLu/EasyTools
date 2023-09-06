using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTools.InternalComponent;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace EasyTools {

	/// <summary>
	/// 游戏运行时始终不被摧毁的 MonoBehaviour
	/// </summary>
	public partial class EasyToolsGameObject : MonoBehaviour {
		private static EasyToolsGameObject _instance;
		public static EasyToolsGameObject Instance {
			get {
				if (_instance == null) {
					var obj = Instantiate(Resources.Load<GameObject>("EasyToolsGameObject"));
					DontDestroyOnLoad(obj);
					_instance = obj.GetComponent<EasyToolsGameObject>();
				}
				return _instance;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RuntimeInit() {
			Instance.enabled = true;
		}

		private void Awake() {
			InitScrollMsg();
			InitGameSettingsPanel();
		}

		#region ScrollMsg

		[Header("ScrollMsg")]
		[SerializeField] private VerticalLayoutGroup m_scrollMsgHolder;
		[SerializeField] private GameObject m_scrollMsgPrefab;
		private const float MessageFadeOutTime = 1f;
		private const float ScrollTime = 0.5f;

		private float _height;
		private Vector2 _defaultPos;
		private RectTransform _rect;

		private void InitScrollMsg() {
			_height = m_scrollMsgHolder.spacing + m_scrollMsgPrefab.transform.ToRect().rect.height;
			_rect = m_scrollMsgHolder.transform as RectTransform;
			_defaultPos = _rect.anchoredPosition;
		}

		Coroutine _scrollCoroutine;
		internal void ShowMsg(string message, float duration = 3f, uint color = ColorU.white) {
			if (message == null) return;

			Instantiate(m_scrollMsgPrefab, _rect).GetComponent<ScrollItem>().Show(message, ScrollTime, duration, MessageFadeOutTime, ColorU.Parse(color));

			_rect.anchoredPosition -= new Vector2(0, _height);

			var startPos = _rect.anchoredPosition;
			EasyTween.Linear(ScrollTime, d => _rect.anchoredPosition = Vector2.Lerp(startPos, _defaultPos, d)).RunOn(this, ref _scrollCoroutine);
		}

		#endregion

		public static void MainMenuOrExit() {
			if (SceneManager.GetActiveScene().buildIndex == 0) {
				GameAudio.FadeBGM(null, 0.4f);
				TransMask.ShowMask().Yield(() => {
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#else
					Application.Quit();
#endif
				}).RunOn(Instance);
			}
			else TransMask.LoadScene(0);
		}
	}
}
