using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {

	public static class UnityEditorExtensions {
		[MenuItem("CONTEXT/Transform/重置位置（不影响子物体）", false, 200)]
		public static void ResetPosKeepingChildren(MenuCommand command) {
			var transform = (Transform)command.context;
			var children = transform.GetChildren();
			var childPos = children.Select(child => (child.position, child.rotation)).ToArray();
			Undo.RecordObjects(children.Append(transform).ToArray(), "Reset position keeping children");
			transform.localPosition = Vector3.zero;
			children.ForEach((child, index) => {
				child.position = childPos[index].position;
				child.rotation = childPos[index].rotation;
			});
		}

		[MenuItem("CONTEXT/Transform/重置旋转（不影响子物体）", false, 200)]
		public static void ResetRotKeepingChildren(MenuCommand command) {
			var transform = (Transform)command.context;
			var children = transform.GetChildren();
			var childPos = children.Select(child => (child.position, child.rotation)).ToArray();
			Undo.RecordObjects(children.Append(transform).ToArray(), "Reset rotation keeping children");
			transform.rotation = Quaternion.identity;
			children.ForEach((child, index) => {
				child.position = childPos[index].position;
				child.rotation = childPos[index].rotation;
			});
		}
	}
}
