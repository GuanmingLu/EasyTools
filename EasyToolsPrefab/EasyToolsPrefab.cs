using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTools.InternalComponent;

namespace EasyTools {

	/// <summary>
	/// 游戏运行时始终不被摧毁的 MonoBehaviour
	/// </summary>
	public class EasyToolsPrefab : MonoBehaviour {
		private static EasyToolsPrefab _instance;
		public static EasyToolsPrefab Instance {
			get {
				if (_instance == null) {
					var prefab = Instantiate(Resources.Load<GameObject>("EasyToolsPrefab"));
					DontDestroyOnLoad(prefab);
					_instance = prefab.GetComponent<EasyToolsPrefab>();
				}
				return _instance;
			}
		}

		private static TransitionMaskComponent _transitionMaskComponent;
		internal static TransitionMaskComponent TransitionMaskComponent {
			get {
				if (_transitionMaskComponent == null) {
					_transitionMaskComponent = Instance.GetComponentInChildren<TransitionMaskComponent>();
				}
				return _transitionMaskComponent;
			}
		}

		private static ScrollMessageComponent _scrollMessageComponent;
		internal static ScrollMessageComponent ScrollMessageComponent {
			get {
				if (_scrollMessageComponent == null) {
					_scrollMessageComponent = Instance.GetComponentInChildren<ScrollMessageComponent>();
				}
				return _scrollMessageComponent;
			}
		}

		private static GameAudioComponent _gameAudioComponent;
		internal static GameAudioComponent GameAudioComponent {
			get {
				if (_gameAudioComponent == null) {
					_gameAudioComponent = Instance.GetComponentInChildren<GameAudioComponent>();
				}
				return _gameAudioComponent;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Startup() => Instance.enabled = true;
	}
}
