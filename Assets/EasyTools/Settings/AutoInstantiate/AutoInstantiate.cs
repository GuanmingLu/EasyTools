using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyTools.Settings {

	internal class AutoInstantiate : ResourceSingleton<AutoInstantiate> {
		[SerializeField] private GameObject[] _dontDestroyOnLoad, _instantiateOnSceneLoaded;


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			Instance?.InstantiateOnStartup();
		}

		private void InstantiateOnStartup() {
			SceneManager.sceneLoaded += InstantiateOnSceneLoaded;

			foreach (var item in _dontDestroyOnLoad) {
				if (item != null) DontDestroyOnLoad(Instantiate(item));
			}
		}

		private void InstantiateOnSceneLoaded(Scene scene, LoadSceneMode mode) {
			foreach (var item in _instantiateOnSceneLoaded) {
				if (item != null) Instantiate(item);
			}
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("EasyTools/Auto Instantiate")]
		private static void Menu() {
			UnityEditor.EditorGUIUtility.PingObject(Instance);
		}
#endif
	}

}
