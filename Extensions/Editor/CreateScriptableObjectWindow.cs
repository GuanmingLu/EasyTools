using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;

namespace EasyTools {
	public class CreateScriptableObjectWindow : EditorWindow {

		private string _menuBtnName = "选择一个 ScriptableObject";
		private GenericMenu _menu;
		private Type _selected = null;
		private string _path = "Assets";


		[MenuItem("EasyTools/创建 ScriptableObject")]
		static void Init() {
			var window = EditorWindow.GetWindow<CreateScriptableObjectWindow>(false, "创建 ScriptableObject", true);
			window.Show();
		}

		private void OnEnable() {
			_menu = new GenericMenu();

			// 获取所有 ScriptableObject 的子类
			var allTypes = TypeCache.GetTypesDerivedFrom<ScriptableObject>().Where(
				t => !t.IsSubclassOf(typeof(UnityEditor.Editor)) && !t.IsSubclassOf(typeof(EditorWindow))
			);

			foreach (var t in allTypes) {
				var asmName = t.Assembly.GetName().Name;
				var root = asmName.StartsWith("Assembly-CSharp") ? asmName : $"Others/{asmName}";
				_menu.AddItem(new GUIContent($"{root}/{t.FullName.Replace('.', '/')}"), false, SelectType, t);
			}

			UseSelectedPath();
		}

		private void SelectType(object type) {
			_selected = type as Type;
			_menuBtnName = _selected.Name;
		}

		private void OnGUI() {
			var disabled = false;
			var createBtnName = "创建资源文件";

			if (GUILayout.Button(_menuBtnName)) {
				_menu.ShowAsContext();
			}
			if (_selected == null) {
				disabled = true;
				createBtnName = "请选择要创建的 ScriptableObject";
			}

			GUILayout.Space(10f);

			_path = EditorGUILayout.TextField("保存路径", _path);
			if (GUILayout.Button("使用当前选定的路径")) {
				UseSelectedPath();
			}
			if (!AssetDatabase.IsValidFolder(_path)) {
				disabled = true;
				createBtnName = "给定的路径无效！";
			}

			GUILayout.Space(10f);

			using (new EditorGUI.DisabledScope(disabled)) {
				// 绘制创建按钮并检测用户点击事件
				if (GUILayout.Button(createBtnName)) {
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

		private void UseSelectedPath() {
			if (Selection.assetGUIDs.Length == 1) {
				var p = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
				// 只有以 Assets 开头的路径才合法
				if (p.StartsWith("Assets")) {
					// 如果有扩展名（选中的是文件）则选择其文件夹
					_path = Path.HasExtension(p) ? Path.GetDirectoryName(p) : p;
				}
			}
		}
	}
}
