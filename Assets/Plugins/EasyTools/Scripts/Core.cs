using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyTools.Settings {
	public class ResourceSingleton<T> : ScriptableObject where T : ScriptableObject {
		private static string _fileName => $"DontRename_{typeof(T).Name}_Singleton";

		private static T _instance;
		protected static T Instance {
			get {
				if (_instance != null) return _instance;
				_instance = Resources.Load<T>(_fileName);
#if UNITY_EDITOR
				if (_instance == null) {
					_instance = ScriptableObject.CreateInstance<T>();
					var dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(_instance)));
					if (!AssetDatabase.IsValidFolder($"{dir}/Resources")) AssetDatabase.CreateFolder(dir, "Resources");
					AssetDatabase.CreateAsset(_instance, $"{dir}/Resources/{_fileName}.asset");
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
