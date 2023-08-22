using System.Collections;
using UnityEngine;
using System;

namespace EasyTools {
	public enum EasingType {
		Linear,
		EaseOut,
		EaseIn,
		EaseInOut,
		EaseOutBack,
	}

	/// <summary>
	/// 缓动类，用于在一段时间内从一个值变化到另一个值
	/// </summary>
	public class Easing {
		const float OUT_C1 = 1.70158f;
		const float OUT_C3 = OUT_C1 + 1;
		/// <summary>
		/// 输入0-1，根据缓动方式计算输出0-1
		/// </summary>
		private static float Evaluate(EasingType easingType, float x) => easingType switch {
			EasingType.Linear => x,
			EasingType.EaseOut => 1 - Mathf.Pow(1 - x, 3),
			EasingType.EaseIn => x * x,
			EasingType.EaseInOut => x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2,
			EasingType.EaseOutBack => 1 + OUT_C3 * Mathf.Pow(x - 1, 3) + OUT_C1 * Mathf.Pow(x - 1, 2),
			_ => throw new NotImplementedException(),
		};

		private float _startValue;
		private float _targetValue;
		private EasingType _easingType;

		private MonoBehaviour _runner;
		private Coroutine _coroutine;
		public Coroutine Coroutine => _coroutine;

		public Easing(MonoBehaviour runner, float start, float end, EasingType easingType) {
			_runner = runner;
			_startValue = start;
			_targetValue = end;
			_easingType = easingType;
		}

		private event Action<float> _onValueChanged;
		public Easing OnValueChanged(Action<float> onValueChanged) {
			_onValueChanged += onValueChanged;
			return this;
		}

		private float _currentTime;
		private float CurrentTime {
			get => _currentTime;
			set {
				_currentTime = Mathf.Clamp(value, 0, _totalDuration);
				_onValueChanged?.Invoke(_startValue + (_targetValue - _startValue) * Evaluate(_easingType, _currentTime / _totalDuration));
			}
		}

		private float _totalDuration = 0.5f;
		public Easing SetDuration(float totalDuration) {
			_currentTime = _currentTime * totalDuration / _totalDuration;
			_totalDuration = totalDuration;
			return this;
		}

		private event Action _onStop;

		/// <summary>
		/// 停止当前缓动 <br/>
		/// 会触发 <see cref="OnStop"/>
		/// </summary>
		public void Stop() {
			if (_coroutine != null) {
				_runner.StopCoroutine(ref _coroutine);
				_onStop?.Invoke();
			}
		}
		public Easing OnStop(Action onStop) {
			_onStop += onStop;
			return this;
		}

		private event Action _onStartForward, _onFinishForward;

		/// <summary>
		/// 从当前数值开始正放 <br/>
		/// 会触发 <see cref="OnStartForward"/> 和 <see cref="OnFinishForward"/>
		/// </summary>
		public Easing RunForward(Action onFinished = null) {
			IEnumerator C() {
				CurrentTime = CurrentTime;
				while (CurrentTime < _totalDuration) {
					yield return null;
					CurrentTime += Time.deltaTime;
				}
				onFinished?.Invoke();
				_onFinishForward?.Invoke();
				_coroutine = null;
			}
			_onStartForward?.Invoke();
			C().RunOn(_runner, ref _coroutine);
			return this;
		}
		public Easing OnStartForward(Action onStartForward) {
			_onStartForward += onStartForward;
			return this;
		}
		public Easing OnFinishForward(Action onFinishForward) {
			_onFinishForward += onFinishForward;
			return this;
		}

		private event Action _onStartBackward, _onFinishBackward;

		/// <summary>
		/// 从当前数值开始倒放 <br/>
		/// 会触发 <see cref="OnStartBackward"/> 和 <see cref="OnFinishBackward"/>
		/// </summary>
		public Easing RunBackward(Action onFinished = null) {
			IEnumerator C() {
				CurrentTime = CurrentTime;
				while (CurrentTime > 0) {
					yield return null;
					CurrentTime -= Time.deltaTime;
				}
				onFinished?.Invoke();
				_onFinishBackward?.Invoke();
				_coroutine = null;
			}
			_onStartBackward?.Invoke();
			C().RunOn(_runner, ref _coroutine);
			return this;
		}
		public Easing OnStartBackward(Action onStartBackward) {
			_onStartBackward += onStartBackward;
			return this;
		}
		public Easing OnFinishBackward(Action onFinishBackward) {
			_onFinishBackward += onFinishBackward;
			return this;
		}

		/// <summary>
		/// 将当前数值设置为 0 <br/>
		/// 会触发 <see cref="OnValueChanged"/> 和 <see cref="OnFinishBackward"/>
		/// </summary>
		public void ClearValue() {
			_runner.StopCoroutine(ref _coroutine);
			CurrentTime = 0;
			_onFinishBackward?.Invoke();
		}
	}
}
