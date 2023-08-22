using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Mathf;

namespace EasyTools {

	/// <summary>
	/// 渐变
	/// </summary>
	public static class EasyTween {

		public static IEnumerator Interpolate(float time, Action<float> action) {
			float t = 0;
			while (t < time) {
				action(t / time);
				yield return null;
				t += Time.deltaTime;
			}
			action(1);
		}

		public static IEnumerator Linear(float time, Action<float> action, float start = 0, float end = 1)
			=> Interpolate(time, t => action(start + (end - start) * t));

		public static IEnumerator EaseIn(float time, Action<float> action, float start = 0, float end = 1)
			=> Interpolate(time, t => action(start + (end - start) * (1 - Cos(PI * t / 2))));

		public static IEnumerator EaseOut(float time, Action<float> action, float start = 0, float end = 1)
			=> Interpolate(time, t => action(start + (end - start) * Sin(PI * t / 2)));

		public static IEnumerator EaseInOut(float time, Action<float> action, float start = 0, float end = 1)
			=> Interpolate(time, t => action(start + (end - start) * (1 - Cos(t * PI)) / 2));
	}
}
