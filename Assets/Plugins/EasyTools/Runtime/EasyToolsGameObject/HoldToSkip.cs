using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools {
	partial class EasyToolsGameObject {
		[Header("HoldToSkip")]
		[SerializeField] internal CanvasGroup m_holdToSkip;
		[SerializeField] internal Image m_holdToSkipFill;
	}

	public static class HoldToSkip {
		private static EasyToolsGameObject Instance => EasyToolsGameObject.Instance;

		private static Easing _show = new Easing(Instance, 0, 1, EasingType.Linear)
			.SetDuration(0.2f)
			.OnValueChanged(v => Instance.m_holdToSkip.alpha = v);

		private static Easing _fill = new Easing(Instance, 0, 1, EasingType.Linear)
			.OnValueChanged(v => Instance.m_holdToSkipFill.fillAmount = v)
			.OnStartForward(() => _show.RunForward())
			.OnFinishBackward(() => _show.RunBackward());

		private static bool IsHolding => Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return);

		private static Coroutine _coroutine;
		public static void Start(Action onSkip, float seconds = 1.5f) {
			_fill.SetDuration(seconds);
			IEnumerator C() {
				yield return Wait.Seconds(0.5f);    // 0.5s后才开始判定（避免按下交互时触发跳过判定）

				bool _forwarding = false;
				while (true) {
					yield return null;
					if (!_forwarding && IsHolding) {
						_forwarding = true;
						_fill.RunForward(onSkip);
					}
					if (_forwarding && !IsHolding) {
						_forwarding = false;
						_fill.RunBackward();
					}
				}
			}
			C().RunOn(Instance, ref _coroutine);
		}
		public static void Stop() {
			Instance.StopCoroutine(ref _coroutine);
			_fill.ClearValue();
		}
	}
}
