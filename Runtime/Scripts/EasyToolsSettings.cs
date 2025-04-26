using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyTools.Settings {

	public class EasyToolsSettings : ScriptableObject {
		private static EasyToolsSettings _instance;
		private static EasyToolsSettings Instance {
			get {
				if (_instance != null) return _instance;
				_instance = Resources.Load<EasyToolsSettings>("EasyToolsSettings");
#if UNITY_EDITOR
				if (_instance == null) {
					_instance = CreateInstance<EasyToolsSettings>();
					var path = UnityExtensions.Editor.EnsureFolder("Assets", "Plugins", "EasyTools", "Resources") + "/EasyToolsSettings.asset";
					AssetDatabase.CreateAsset(_instance, path);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					EditorUtility.SetDirty(_instance);
				}
#endif
				return _instance;
			}
		}

		#region 滚动消息

		[Header("滚动消息")]
		[SerializeField] private bool m_showScrollLog;

		public static bool ShowScrollLog {
			get => Instance.m_showScrollLog;
			set => Instance.m_showScrollLog = value;
		}

		#endregion

		#region 对话

		[Header("对话")]
		[SerializeField] private SerializableDictionary<string, Sprite> m_avatarMap;
		[SerializeField] private float m_characterInterval = 0.02f;
		[SerializeField] private KeyCode[] m_nextKeys = { KeyCode.Space, KeyCode.Mouse0 };

		public static bool TryGetAvatar(string key, out Sprite avatar) => Instance.m_avatarMap.TryGetValue(key, out avatar);
		public static float CharInterval => Instance.m_characterInterval;
		public static bool NextInput => Instance.m_nextKeys.Any(k => Input.GetKeyDown(k));

		#endregion

		#region 自动初始化

		[Header("自动初始化")]
		[SerializeField] private GameObject[] m_dontDestroyOnLoad;
		[SerializeField] private GameObject[] m_onSceneLoaded;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InstantiateOnStartup() {
			SceneManager.sceneLoaded += Instance.InstantiateOnSceneLoaded;

			foreach (var item in Instance.m_dontDestroyOnLoad) {
				if (item != null) DontDestroyOnLoad(Instantiate(item));
			}
		}

		private void InstantiateOnSceneLoaded(Scene scene, LoadSceneMode mode) {
			foreach (var item in m_onSceneLoaded) {
				if (item != null) Instantiate(item);
			}
		}

		#endregion

		#region 静态引用

		[Header("静态引用")]
		[SerializeField] private StaticReferences m_staticReferences;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void ApplyStaticReferences() => Instance.m_staticReferences.SetAllValues();

		#endregion

#if UNITY_EDITOR
		[MenuItem("EasyTools/Settings", false, int.MaxValue)]
		private static void Menu() {
			EditorGUIUtility.PingObject(Instance);
		}
#endif
	}
}
