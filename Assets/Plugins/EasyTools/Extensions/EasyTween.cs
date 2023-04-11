using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Mathf;

namespace EasyTools {

	/// <summary>
	/// 渐变
	/// </summary>
	public static class EasyTween {
		/// <summary>
		/// 线性渐变
		/// </summary>
		/// <param name="time">总时间</param>
		/// <param name="action">每帧使用当前值（0 -> 1）调用</param>
		/// <param name="decrease">若为true，则使用1减当前值调用</param>
		public static IEnumerator Linear(float time, Action<float> action) {
			float t = 0;
			while (t < time) {
				action(t / time);
				yield return null;
				t += Time.deltaTime;
			}
			action(1);
		}
		/// <summary>
		/// 递增渐变（变化速度先慢后快）
		/// </summary>
		/// <param name="time">总时间</param>
		/// <param name="action">每帧使用当前值（0 -> 1）调用</param>
		/// <param name="decrease">若为true，则使用1减当前值调用</param>
		public static IEnumerator EaseIn(float time, Action<float> action) {
			float t = 0;
			while (t < time) {
				action(1 - Cos(PI * t / time / 2));
				yield return null;
				t += Time.deltaTime;
			}
			action(1);
		}
		/// <summary>
		/// 递减渐变（变化速度先快后慢）
		/// </summary>
		/// <param name="time">总时间</param>
		/// <param name="action">每帧使用当前值（0 -> 1）调用</param>
		/// <param name="decrease">若为true，则使用1减当前值调用</param>
		public static IEnumerator EaseOut(float time, Action<float> action) {
			float t = 0;
			while (t < time) {
				action(Sin(PI * t / time / 2));
				yield return null;
				t += Time.deltaTime;
			}
			action(1);
		}
		/// <summary>
		/// 非线性渐变（变化速度慢-快-慢）
		/// </summary>
		/// <param name="time">总时间</param>
		/// <param name="action">每帧使用当前值（0 -> 1）调用</param>
		/// <param name="decrease">若为true，则使用1减当前值调用</param>
		public static IEnumerator EaseInOut(float time, Action<float> action) {
			float t = 0;
			while (t < time) {
				action(-(Cos(PI * t / time) - 1) / 2);
				yield return null;
				t += Time.deltaTime;
			}
			action(1);
		}
	}

}
