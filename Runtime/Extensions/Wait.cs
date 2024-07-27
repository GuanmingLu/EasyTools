using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyTools {
	public interface IPauseHandler {
		public bool IsPaused { get; }
	}
	public class PauseToken : IPauseHandler {
		public bool IsPaused { get; private set; }
		public void Pause() => IsPaused = true;
		public void Resume() => IsPaused = false;
	}
	public class PauseTokens : IPauseHandler {
		private HashSet<IPauseHandler> _tokens;
		public bool IsPaused => _tokens.Any(t => t.IsPaused);

		public PauseTokens(params IPauseHandler[] tokens) => _tokens = new(tokens);

		public void Add(IPauseHandler token) => _tokens.Add(token);
		public void Remove(IPauseHandler token) => _tokens.Remove(token);
		public void Clear() => _tokens.Clear();
	}


	public static class Wait {
		public static readonly WaitForEndOfFrame EndOffFrame = new();

		private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new();
		public static WaitForSeconds Seconds(float seconds) {
			if (!_waitForSeconds.ContainsKey(seconds)) _waitForSeconds[seconds] = new(seconds);
			return _waitForSeconds[seconds];
		}

		private static readonly Dictionary<float, WaitForSecondsRealtime> _waitForSecondsRealtime = new();
		public static WaitForSecondsRealtime SecondsUnscaled(float seconds) {
			if (!_waitForSecondsRealtime.ContainsKey(seconds)) _waitForSecondsRealtime[seconds] = new(seconds);
			return _waitForSecondsRealtime[seconds];
		}

		public static IEnumerator Seconds(float seconds, Func<bool> interruption, bool unscaledTime = false) {
			float time = 0;
			while (time < seconds) {
				time += (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
				if (interruption()) break;
				yield return null;
			}
		}
		public static IEnumerator Seconds(float seconds, IPauseHandler pauseHandler) {
			float time = 0;
			while (time < seconds) {
				yield return null;
				if (!pauseHandler.IsPaused) time += Time.deltaTime;
			}
		}

		public static IEnumerator NextFrame(IPauseHandler pauseHandler) {
			yield return null;
			while (pauseHandler.IsPaused) yield return null;
		}

		public static IEnumerator Frames(int count) {
			for (int i = 0; i < count; i++) {
				yield return null;
			}
		}

		/// <summary>
		/// 等待直到条件为true
		/// </summary>
		/// <param name="condition">条件</param>
		public static IEnumerator Until(Func<bool> condition) {
			while (!condition()) yield return null;
		}
		/// <summary>
		/// 等待直到条件为true
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="timeout">超时（大于0时有效）</param>
		public static IEnumerator Until(Func<bool> condition, float timeout = -1) => Until(condition, null, timeout);
		/// <summary>
		/// 等待直到条件为true
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="then">完成后的回调（参数为花费的时间）</param>
		/// <param name="timeout">超时（大于0时有效）</param>
		public static IEnumerator Until(Func<bool> condition, Action<float> then, float timeout = -1) {
			var t = 0f;
			while (!condition()) {
				yield return null;
				t += Time.deltaTime;
				if (t >= timeout && timeout >= 0) break;
			}
			then?.Invoke(t);
		}
	}

}
