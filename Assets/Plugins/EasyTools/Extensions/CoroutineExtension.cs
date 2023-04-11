using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

	public class CoroutineHandler : CustomYieldInstruction {
		public static CoroutineHandler Start(MonoBehaviour target, IEnumerator coroutine) {
			var c = new CoroutineHandler();
			c._target = target;
			c._coroutine = target.StartCoroutine(c.Run(coroutine));
			return c;
		}

		public static CoroutineHandler Start(MonoBehaviour target, YieldInstruction coroutine) {
			var c = new CoroutineHandler();
			c._target = target;
			c._coroutine = target.StartCoroutine(c.Run(coroutine));
			return c;
		}

		private Coroutine _coroutine;
		private MonoBehaviour _target;
		public bool IsDone { get; private set; }
		public event Action OnFinished;
		public override bool keepWaiting => !IsDone;

		private IEnumerator Run(IEnumerator i) {
			yield return i;
			IsDone = true;
			OnFinished?.Invoke();
		}

		private IEnumerator Run(YieldInstruction y) {
			yield return y;
			IsDone = true;
			OnFinished?.Invoke();
		}

		public void Stop() {
			_target.StopCoroutine(_coroutine);
			IsDone = true;
		}
	}

	public static class CoroutineExtension {
		public static CoroutineHandler Run(this YieldInstruction coroutine, MonoBehaviour self) => CoroutineHandler.Start(self, coroutine);
		public static CoroutineHandler Run(this IEnumerator coroutine, MonoBehaviour self) => CoroutineHandler.Start(self, coroutine);
		public static CoroutineHandler Run(this YieldInstruction coroutine) => CoroutineHandler.Start(EasyToolsPrefab.Instance, coroutine);
		public static CoroutineHandler Run(this IEnumerator coroutine) => CoroutineHandler.Start(EasyToolsPrefab.Instance, coroutine);

		public static IEnumerator Then(this IEnumerator a, Action action) { yield return a; action(); }
		public static IEnumerator Then(this IEnumerator a, IEnumerator next) { yield return a; yield return next; }
		public static IEnumerator Then(this IEnumerator a, YieldInstruction next) { yield return a; yield return next; }
		public static IEnumerator Then(this YieldInstruction a, Action action) { yield return a; action(); }
		public static IEnumerator Then(this YieldInstruction a, IEnumerator next) { yield return a; yield return next; }
		public static IEnumerator Then(this YieldInstruction a, YieldInstruction next) { yield return a; yield return next; }

		public static CoroutineHandler Loop(this MonoBehaviour self, float interval, Action func, float delay = 0f) {
			IEnumerator C() {
				yield return Wait.Seconds(delay);
				while (true) {
					func?.Invoke();
					yield return Wait.Seconds(interval);
				}
			}
			return C().Run(self);
		}
		public static CoroutineHandler Loop(this MonoBehaviour self, Action func, float delay = 0f) {
			IEnumerator C() {
				yield return Wait.Seconds(delay);
				while (true) {
					yield return null;
					func?.Invoke();
				}
			}
			return C().Run(self);
		}

	}
}
