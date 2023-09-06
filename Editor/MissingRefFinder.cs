using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EasyTools;
using EasyTools.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyTools.Editor {

	public class MissingReferencesFinder : MonoBehaviour {

		private static IEnumerable<GameObject> GetSceneObjects()
			=> Resources.FindObjectsOfTypeAll<GameObject>().Where(
				go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go)) && go.hideFlags == HideFlags.None
			);

		[MenuItem("EasyTools/工具/查找场景内丢失的引用", false, 50)]
		public static void FindMissingReferencesInCurrentScene() {
			RefreshGUIDMap();
			FindMissingReferences(SceneManager.GetActiveScene().path, GetSceneObjects());
		}

		// [MenuItem("Tools/Show Missing Object References in assets", false, 52)]
		// public static void MissingSpritesInAssets() {
		// 	var allAssets = AssetDatabase.GetAllAssetPaths();
		// 	var objs = allAssets.Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(a => a != null).ToArray();

		// 	FindMissingReferences("Project", objs);
		// }

		private static void FindMissingReferences(string context, IEnumerable<GameObject> objects) {
			foreach (var go in objects) {
				var components = go.GetComponents<Component>();

				foreach (var c in components) {
					if (!c) {
						SerializedObject sObj = new(go);
						sObj.Reflect().TrySet("inspectorMode", InspectorMode.Debug);
						var fileId = sObj.FindProperty("m_LocalIdentfierInFile").intValue.ToString();
						if (_guidOfFileId.ContainsKey(fileId))
							Debug.LogError($"{FullPath(go)} : Missing: {_guidOfFileId[fileId].ToJson()}", go);
						else
							Debug.LogError($"Missing Component in GO: {FullPath(go)} <{fileId}>", go);
						continue;
					}

					SerializedObject so = new SerializedObject(c);
					var sp = so.GetIterator();

					while (sp.NextVisible(true)) {
						if (sp.propertyType == SerializedPropertyType.ObjectReference) {
							if (sp.objectReferenceValue == null
								&& sp.objectReferenceInstanceIDValue != 0) {
								ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
							}
						}
					}
				}
			}
		}

		private const string err = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";

		private static void ShowError(string context, GameObject go, string c, string property) {
			Debug.LogError(string.Format(err, FullPath(go), c, property, context), go);
		}

		private static string FullPath(GameObject go) {
			return go.transform.parent == null
				? go.name
					: FullPath(go.transform.parent.gameObject) + "/" + go.name;
		}

		private static Dictionary<string, HashSet<string>> _guidOfFileId = new();
		private static Regex _scriptReg = new(@"  m_Script: \{fileID: 11500000, guid: (.+), type: 3\}", RegexOptions.Compiled);
		public static void RefreshGUIDMap() {
			_guidOfFileId.Clear();
			var lines = File.ReadAllLines(Path.GetFullPath(SceneManager.GetActiveScene().path));
			foreach (var (line, index) in lines.WithIndex()) {
				if (_scriptReg.TryMatch(line, out var match)) {
					var guid = match.Groups[1].Value;
					if (!string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid))) continue;
					var fileId = GetObjectFileId(lines, index);
					if (!_guidOfFileId.ContainsKey(fileId)) _guidOfFileId.Add(fileId, new());
					_guidOfFileId[fileId].Add(guid);
				}
			}
		}

		private static string GetObjectFileId(string[] lines, int scriptLineIdx) {
			Regex objReg = new(@"m_GameObject: \{fileID: (.+)\}");
			for (int i = scriptLineIdx - 1; i >= 0 && lines[i].StartsWith("  "); i--) {
				if (objReg.TryMatch(lines[i], out var match)) return match.Groups[1].Value;
			}
			return "?";
		}

	}
}
