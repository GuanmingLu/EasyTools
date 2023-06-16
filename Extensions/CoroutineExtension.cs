using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

	/// <summary>
	/// 对于协程状态的封装，可以监测协程是否结束，以及在协程结束时触发事件
	/// </summary>
	public class CoroutineState : CustomYieldInstruction {

		/// <summary>
		/// 对于协程状态的封装，可以监测协程是否结束，以及在协程结束时触发事件
		/// </summary>
		public CoroutineState() { }

		/// <summary>
		/// 协程是否结束
		/// </summary>
		public bool IsDone { get; protected set; } = true;
		public override bool keepWaiting => !IsDone;

		/// <summary>
		/// 协程结束时触发的事件
		/// </summary>
		public event Action OnFinished;
		protected void InvokeOnFinished() => OnFinished?.Invoke();
	}

	/// <summary>
	/// 倒计时
	/// </summary>
	public class CountDown : CoroutineState {
		private WaitForSeconds _wait;
		private CountDown() { }

		/// <summary>
		/// 开始倒计时
		/// </summary>
		public static CountDown Start(float time) {
			CountDown countDown = new CountDown() { _wait = new WaitForSeconds(time) };
			countDown.Reset();
			return countDown;
		}

		/// <summary>
		/// 重新开始倒计时
		/// </summary>
		new public void Reset() {
			IEnumerator C() {
				IsDone = false;
				yield return _wait;
				IsDone = true;
				InvokeOnFinished();
			}
			EasyToolsPrefab.Instance.StartCoroutine(C());
		}
	}

	public static class CoroutineExtension {
		public static IEnumerator Expand(this IEnumerator func) {
			while (func.MoveNext()) {
				if (func.Current is IEnumerator sub) {
					var subFunc = sub.Expand();
					while (subFunc.MoveNext()) yield return subFunc.Current;
				}
				else {
					yield return func.Current;
				}
			}
		}

		public static Coroutine RunOn(this IEnumerator func, MonoBehaviour runner) => runner.StartCoroutine(func.Expand());
		public static void Run(this IEnumerator func) => EasyToolsPrefab.Instance.StartCoroutine(func.Expand());

		public static void RunOn(this IEnumerator func, MonoBehaviour runner, ref Coroutine coroutine, bool stopOld = true) {
			if (stopOld && coroutine != null) runner.StopCoroutine(coroutine);
			coroutine = runner.StartCoroutine(func.Expand());
		}
		public static void StopCoroutine(this MonoBehaviour runner, ref Coroutine coroutine) {
			if (coroutine != null) {
				runner.StopCoroutine(coroutine);
				coroutine = null;
			}
		}

		public static IEnumerator Then(this IEnumerator func, Action action) {
			yield return func;
			action();
		}
	}
}
