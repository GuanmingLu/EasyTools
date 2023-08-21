using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyTools.Settings {
	public class ResourceSingleton<T> : ScriptableObject where T : ResourceSingleton<T> {
		private static string FileName => $"{typeof(T).Namespace}.{typeof(T).Name}";

		private static T _instance;
		protected static T Instance {
			get {
				if (_instance != null) return _instance;
				_instance = Resources.Load<T>(FileName);
#if UNITY_EDITOR
				if (_instance == null) {
					_instance = CreateInstance<T>();
					var dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(_instance)));
					if (!AssetDatabase.IsValidFolder($"{dir}/Resources")) AssetDatabase.CreateFolder(dir, "Resources");
					AssetDatabase.CreateAsset(_instance, $"{dir}/Resources/{FileName}.asset");
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					EditorUtility.SetDirty(_instance);
				}
#endif
				return _instance;
			}
		}
	}
}
