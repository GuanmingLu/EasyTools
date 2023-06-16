using System.Linq;
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

		public static void SetZDeg(this Transform self, float z) {
			var v = self.eulerAngles; v.z = z; self.eulerAngles = v;
		}
		public static void SetZDeg(this Quaternion self, float z) {
			var v = self.eulerAngles; v.z = z; self.eulerAngles = v;
		}

		public static void SetScale(this Transform self, float scale) {
			self.localScale = new Vector3(scale, scale, scale);
		}

		public static IEnumerable<Transform> GetChildren(this Transform self) => Enumerable.Range(0, self.childCount).Select(i => self.GetChild(i));
		public static IEnumerable<Transform> ReversedChildren(this Transform self) {
			for (var i = self.childCount - 1; i >= 0; i--) {
				yield return self.GetChild(i);
			}
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
	}
}
