using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.Settings {

	internal class InternalSettings : ResourceSingleton<InternalSettings> {
		[SerializeField] private EasyToolsSettings m_settings;
		internal static EasyToolsSettings Settings => Instance.m_settings;

		[SerializeField] private EasyToolsPrefab m_EasyToolsPrefab;

		private static EasyToolsPrefab _easyToolsGameObject;
		internal static EasyToolsPrefab EasyToolsGameObject {
			get {
				if (_easyToolsGameObject == null) {
					var obj = Instantiate(Instance.m_EasyToolsPrefab.gameObject);
					DontDestroyOnLoad(obj);
					_easyToolsGameObject = obj.GetComponent<EasyToolsPrefab>();
				}
				return _easyToolsGameObject;
			}
		}


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			EasyToolsGameObject.enabled = true;
		}
	}
}
