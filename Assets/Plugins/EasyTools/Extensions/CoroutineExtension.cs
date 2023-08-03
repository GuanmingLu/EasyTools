using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

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
		public static Coroutine Run(this IEnumerator func) => func.RunOn(EasyToolsGameObject.Instance);

		public static void RunOn(this IEnumerator func, MonoBehaviour runner, ref Coroutine coroutine, bool stopOld = true) {
			if (stopOld && coroutine != null) runner.StopCoroutine(coroutine);
			coroutine = runner.StartCoroutine(func.Expand());
		}
		public static void Run(this IEnumerator func, ref Coroutine coroutine, bool stopOld = true)
			=> func.RunOn(EasyToolsGameObject.Instance, ref coroutine, stopOld);

		public static void StopCoroutine(this MonoBehaviour runner, ref Coroutine coroutine) {
			if (coroutine != null) {
				runner.StopCoroutine(coroutine);
				coroutine = null;
			}
		}

		public static IEnumerator Yield(this IEnumerator func, Action onFinished = null) {
			yield return func;
			onFinished();
		}

		public static IEnumerator Yield(this YieldInstruction instruction, Action onFinished = null) {
			yield return instruction;
			onFinished?.Invoke();
		}

		public static IEnumerator ToCoroutine(Func<YieldInstruction> instruction, Action onFinished = null) {
			yield return instruction();
			onFinished?.Invoke();
		}

		public static IEnumerator WaitAll(params IEnumerator[] funcs) {
			Coroutine[] coroutines = new Coroutine[funcs.Length];
			foreach (var (func, index) in funcs.WithIndex()) {
				func.Run(ref coroutines[index]);
			}
			foreach (var coroutine in coroutines) {
				yield return coroutine;
			}
		}
	}
}
