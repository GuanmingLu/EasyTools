using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace EasyTools.Editor {

	public static class UnityEditorExtensions {
		[MenuItem("CONTEXT/Transform/重置位置（不影响子物体）", false, 200)]
		public static void ResetPosKeepingChildren(MenuCommand command) {
			var transform = (Transform)command.context;
			var children = transform.GetChildren();
			var childPos = children.Select(child => (child.position, child.rotation)).ToArray();
			Utils.ChangedObjects(children.Append(transform).ToArray(), "Reset position keeping children");
			transform.localPosition = Vector3.zero;
			foreach (var (child, i) in children.WithIndex()) {
				child.position = childPos[i].position;
				child.rotation = childPos[i].rotation;
			}
		}

		[MenuItem("CONTEXT/Transform/重置旋转（不影响子物体）", false, 200)]
		public static void ResetRotKeepingChildren(MenuCommand command) {
			var transform = (Transform)command.context;
			var children = transform.GetChildren();
			var childPos = children.Select(child => (child.position, child.rotation)).ToArray();
			Utils.ChangedObjects(children.Append(transform).ToArray(), "Reset rotation keeping children");
			transform.rotation = Quaternion.identity;
			foreach (var (child, i) in children.WithIndex()) {
				child.position = childPos[i].position;
				child.rotation = childPos[i].rotation;
			}
		}

		[MenuItem("EasyTools/运行首个场景", false, 0)]
		public static void StartFromScene0() {
			if (!EditorApplication.isPlaying) {
				EditorPrefs.SetBool("EasyTools.StartFromScene0", true);
				EditorApplication.isPlaying = true;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void LoadFirstScene() {
			if (EditorPrefs.GetBool("EasyTools.StartFromScene0", false)) {
				EditorPrefs.DeleteKey("EasyTools.StartFromScene0");
				EditorSceneManager.LoadScene(0);
			}
		}

		[MenuItem("EasyTools/定位当前场景", false, 1)]
		public static void LocateCurrentScene() => EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path));
	}
}
