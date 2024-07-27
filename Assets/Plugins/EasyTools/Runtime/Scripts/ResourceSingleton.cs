using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyTools {
	public class ResourceSingleton<T> : ScriptableObject where T : ResourceSingleton<T> {
		private static string FileName {
			get {
				var type = typeof(T);
				return string.IsNullOrEmpty(type.Namespace) ? type.Name : $"{type.Namespace}.{type.Name}";
			}
		}

		private static T _instance;
		protected static T Instance {
			get {
				if (_instance != null) return _instance;
				_instance = Resources.Load<T>(FileName);
#if UNITY_EDITOR
				if (_instance == null) {
					_instance = CreateInstance<T>();
					var path = UnityExtensions.Editor.EnsureFolder("Assets", "Resources") + $"/{FileName}.asset";
					path = AssetDatabase.GenerateUniqueAssetPath(path);
					AssetDatabase.CreateAsset(_instance, path);
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
