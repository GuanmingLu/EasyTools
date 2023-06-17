using System.Collections;
using UnityEngine;
using System;

namespace Game {
	public enum EasingType {
		Linear,
		EaseOut,
		EaseOutBack,
	}

	public class Easing {
		const float OUT_C1 = 1.70158f;
		const float OUT_C3 = OUT_C1 + 1;

		private float _startValue;
		private float _targetValue;
		private float _totalDuration;
		private EasingType _easingType;
		private Action<float> _onValueChanged;

		private float _currentTime;
		private float CurrentTime {
			get => _currentTime;
			set {
				_currentTime = Mathf.Clamp(value, 0, _totalDuration);
				var x = _currentTime / _totalDuration;
				_onValueChanged?.Invoke(_startValue + (_targetValue - _startValue) * _easingType switch {
					EasingType.Linear => x,
					EasingType.EaseOut => 1 - Mathf.Pow(1 - x, 3),
					EasingType.EaseOutBack => 1 + OUT_C3 * Mathf.Pow(x - 1, 3) + OUT_C1 * Mathf.Pow(x - 1, 2),
					_ => throw new NotImplementedException(),
				});
			}
		}


		public Easing(MonoBehaviour runner, float start, float end, float totalDuration, EasingType easingType, Action<float> onValueChanged) {
			_runner = runner;
			_startValue = start;
			_targetValue = end;
			_totalDuration = totalDuration;
			_easingType = easingType;
			_onValueChanged = onValueChanged;
		}


		private MonoBehaviour _runner;
		private Coroutine _coroutine;

		private event Action _onStop;
		public void Stop() {
			if (_coroutine != null) {
				_runner.StopCoroutine(_coroutine);
				_coroutine = null;
				_onStop?.Invoke();
			}
		}
		public Easing OnStop(Action onStop) {
			_onStop += onStop;
			return this;
		}

		private event Action _onStartForward, _onFinishForward;
		public void RunForward(Action onFinished = null) {
			IEnumerator C() {
				CurrentTime = CurrentTime;
				while (CurrentTime < _totalDuration) {
					yield return null;
					CurrentTime += Time.deltaTime;
				}
				onFinished?.Invoke();
				_onFinishForward?.Invoke();
			}
			Stop();
			_coroutine = _runner.StartCoroutine(C());
			_onStartForward?.Invoke();
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
		public void RunBackward(Action onFinished = null) {
			IEnumerator C() {
				CurrentTime = CurrentTime;
				while (CurrentTime > 0) {
					yield return null;
					CurrentTime -= Time.deltaTime;
				}
				onFinished?.Invoke();
				_onFinishBackward?.Invoke();
			}
			Stop();
			_coroutine = _runner.StartCoroutine(C());
			_onStartBackward?.Invoke();
		}
		public Easing OnStartBackward(Action onStartBackward) {
			_onStartBackward += onStartBackward;
			return this;
		}
	}
}
