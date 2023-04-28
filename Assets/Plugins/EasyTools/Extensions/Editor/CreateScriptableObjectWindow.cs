using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;

namespace EasyTools.Editor {
	public class CreateScriptableObjectWindow : EditorWindow {

		private GenericMenu _menu;
		private Type _selected = null;
		private string _path;


		[MenuItem("EasyTools/工具/创建 ScriptableObject")]
		static void Init() {
			var window = GetWindow<CreateScriptableObjectWindow>(false, "创建 ScriptableObject", true);
			window.OnShow();
		}

		private void OnShow() {
			var pos = position;
			pos.width = 400;
			pos.height = 200;
			position = pos;

			Show();
		}

		private void OnEnable() {
			_menu = new GenericMenu();

			// 获取所有 ScriptableObject 的子类
			var allTypes = TypeCache.GetTypesDerivedFrom<ScriptableObject>().Where(
				t => !t.IsSubclassOf(typeof(UnityEditor.Editor)) && !t.IsSubclassOf(typeof(EditorWindow))
			).OrderBy(t => t.Assembly.GetName().Name).ThenBy(t => t.FullName);

			foreach (var t in allTypes) {
				var asmName = t.Assembly.GetName().Name;
				var root = asmName.StartsWith("Assembly-CSharp") ? asmName : $"Others";
				_menu.AddItem(new GUIContent($"{root}/{t.FullName.Replace('.', '/')}"), false, SelectType, t);
			}
		}

		private void SelectType(object type) => _selected = type as Type;

		private void OnGUI() {
			using (new Utils.BoxGroupScope("选择 ScriptableObject 类")) {
				if (GUILayout.Button(_selected?.Name ?? "选择")) _menu.ShowAsContext();
			}
			if (_selected == null) return;

			using (new Utils.BoxGroupScope("保存路径"))
				Utils.ChooseProjectPath(ref _path);
			if (!AssetDatabase.IsValidFolder(_path)) return;

			using (new Utils.BoxGroupScope("输出")) {
				if (GUILayout.Button("创建资源文件")) {
					// 创建一个新的 ScriptableObject 实例
					var instance = ScriptableObject.CreateInstance(_selected);
					if (instance != null) {
						AssetDatabase.CreateAsset(instance, AssetDatabase.GenerateUniqueAssetPath(Path.Combine(_path, $"{_selected.Name}.asset")));
						// 刷新 Asset 数据库以显示新创建的资源文件
						AssetDatabase.Refresh();
					}
				}
			}
		}
	}
}
