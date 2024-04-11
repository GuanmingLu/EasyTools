using System.Linq;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

namespace EasyTools {

	/// <summary>
	/// 扩展各种方法便于使用
	/// </summary>
	public static class UnityExtensions {
		public static bool TryGetComponentInChildren<T>(this Component self, out T component) {
			component = self.GetComponentInChildren<T>();
			return component != null;
		}
		public static bool TryGetComponentInChildren<T>(this GameObject self, out T component) {
			component = self.GetComponentInChildren<T>();
			return component != null;
		}

		public static void SetA(this SpriteRenderer self, float a) {
			var c = self.color; c.a = Clamp01(a); self.color = c;
		}
		public static void SetA(this Graphic self, float a) {
			var c = self.color; c.a = Clamp01(a); self.color = c;
		}

		private static Vector3 SetValue(this Vector3 self, float? x = null, float? y = null, float? z = null) {
			if (x is float xVal) self.x = xVal;
			if (y is float yVal) self.y = yVal;
			if (z is float zVal) self.z = zVal;
			return self;
		}

		public static void SetPos(this Transform self, float? x = null, float? y = null, float? z = null) {
			self.position = self.position.SetValue(x, y, z);
		}

		public static void SetLocalPos(this Transform self, float? x = null, float? y = null, float? z = null) {
			self.localPosition = self.localPosition.SetValue(x, y, z);
		}

		public static void SetRot(this Transform self, float? x = null, float? y = null, float? z = null) {
			self.eulerAngles = self.eulerAngles.SetValue(x, y, z);
		}

		public static void SetLocalRot(this Transform self, float? x = null, float? y = null, float? z = null) {
			self.localEulerAngles = self.localEulerAngles.SetValue(x, y, z);
		}

		public static void SetScale(this Transform self, float? x = null, float? y = null, float? z = null) {
			self.localScale = self.localScale.SetValue(x, y, z);
		}

		public static void SetAllScale(this Transform self, float scale) {
			self.localScale = new Vector3(scale, scale, scale);
		}

		public static IEnumerable<Transform> GetChildren(this Transform self) => self.Cast<Transform>();
		public static IEnumerable<Transform> ReversedChildren(this Transform self) {
			for (var i = self.childCount - 1; i >= 0; i--) {
				yield return self.GetChild(i);
			}
		}

		public static void DestroySelf(this UnityEngine.Object self) {
			if (self != null) UnityEngine.Object.Destroy(self);
		}

		public static void DestroySelf(this Component self) {
			if (self != null && self.gameObject != null) UnityEngine.Object.Destroy(self.gameObject);
		}

		public static void DestroyAllChildren(this Transform self) {
			for (int i = self.childCount - 1; i >= 0; i--) {
				if (Application.isPlaying) {
					UnityEngine.Object.Destroy(self.GetChild(i).gameObject);
				}
				else {
					UnityEngine.Object.DestroyImmediate(self.GetChild(i).gameObject);
				}
			}
		}


		public static RectTransform ToRect(this Transform self) => self as RectTransform;
		public static RectTransform GetRect(this GameObject self) => self.transform as RectTransform;

		public static void SetVX(this Rigidbody2D self, float x) {
			var v = self.velocity; v.x = x; self.velocity = v;
		}

		public static Vector2 Pos2(this Transform self) => self.position;
		public static Vector3 To3(this Vector2 self) => self;
		public static Vector2 To2(this Vector3 self) => self;

		public static void Deconstruct(this Vector2 self, out float x, out float y) { x = self.x; y = self.y; }
		public static void Deconstruct(this Vector2Int self, out int x, out int y) { x = self.x; y = self.y; }
		public static void Deconstruct(this Vector3 self, out float x, out float y, out float z) { x = self.x; y = self.y; z = self.z; }
		public static void Deconstruct(this Vector3Int self, out int x, out int y, out int z) { x = self.x; y = self.y; z = self.z; }

		/// <summary>
		/// 如果对象==null，则返回真正的null
		/// </summary>
		public static T NullCheck<T>(this T self) where T : UnityEngine.Object => self == null ? null : self;

#if UNITY_EDITOR

		public static class Editor {
			private static bool IsValidFolder(string path) => UnityEditor.AssetDatabase.IsValidFolder(path);
			private static void CreateFolder(string parentFolder, string newFolderName) => UnityEditor.AssetDatabase.CreateFolder(parentFolder, newFolderName);
			public static string EnsureFolder(params string[] folders) {
				StringBuilder folderPath = new();
				foreach (var (folder, index) in folders.WithIndex()) {
					var parent = folderPath.ToString();
					if (index != 0) folderPath.Append('/');
					folderPath.Append(folder);
					if (!IsValidFolder(folderPath.ToString())) CreateFolder(parent, folder);
				}
				return folderPath.ToString();
			}
		}

#endif
	}
}
