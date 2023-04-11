using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		[SerializeField] public object obj = 15;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Startup() => Instance.enabled = true;
	}
}
