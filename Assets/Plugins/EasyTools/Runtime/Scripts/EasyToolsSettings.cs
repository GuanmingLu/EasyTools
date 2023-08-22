using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyTools.Settings {

	public class EasyToolsSettings : ScriptableObject {
		private static EasyToolsSettings _instance;
		public static EasyToolsSettings Instance {
			get {
				if (_instance != null) return _instance;
				_instance = Resources.Load<EasyToolsSettings>("EasyToolsSettings");
#if UNITY_EDITOR
				if (_instance == null) {
					_instance = CreateInstance<EasyToolsSettings>();
					if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
					AssetDatabase.CreateAsset(_instance, "Assets/Resources/EasyToolsSettings.asset");
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					EditorUtility.SetDirty(_instance);
				}
#endif
				return _instance;
			}
		}


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			Instance.InstantiateOnStartup();
		}


		#region 对话

		[Header("对话")]
		[SerializeField] private SerializableDictionary<string, Sprite> avatarMap;
		public bool TryGetAvatar(string key, out Sprite avatar) => avatarMap.TryGetValue(key, out avatar);

		public float CharInterval => 0.02f;

		public bool NextInput => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);

		#endregion

		#region 自动初始化

		[Header("自动初始化")]
		[SerializeField] private GameObject[] _dontDestroyOnLoad;
		[SerializeField] private GameObject[] _onSceneLoaded;

		private void InstantiateOnStartup() {
			SceneManager.sceneLoaded += InstantiateOnSceneLoaded;

			foreach (var item in _dontDestroyOnLoad) {
				if (item != null) DontDestroyOnLoad(Instantiate(item));
			}
		}

		private void InstantiateOnSceneLoaded(Scene scene, LoadSceneMode mode) {
			foreach (var item in _onSceneLoaded) {
				if (item != null) Instantiate(item);
			}
		}

		#endregion


#if UNITY_EDITOR
		[MenuItem("EasyTools/Settings", false, int.MaxValue)]
		private static void Menu() {
			EditorGUIUtility.PingObject(Instance);
		}
#endif
	}
}
