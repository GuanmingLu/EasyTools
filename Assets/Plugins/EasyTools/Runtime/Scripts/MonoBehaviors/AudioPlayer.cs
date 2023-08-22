using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace EasyTools {

	[RequireComponent(typeof(AudioSource))]
	public class AudioPlayer : MonoBehaviour {
		public AudioSource Source { get; private set; }
		private bool _playOnAwake;
		public bool IsStarted { get; private set; } = false;
		public bool IsPaused { get; private set; } = false;

		public UnityEvent loopPointReached;

		private void OnValidate() {
			SetData();
		}

		private void Awake() {
			SetData();
			Source.playOnAwake = false;
			Source.Stop();
		}

		private void Start() {
			if (_playOnAwake) Play();
		}

		private void SetData() {
			Source = GetComponent<AudioSource>();
			_playOnAwake = Source.playOnAwake;
		}

		Coroutine _timerCoroutine;

		public void Play() {
			if (IsStarted) {
				if (IsPaused) {
					Source.UnPause();
				}
				else return;
			}
			else {
				Source.Play();
			}

			IEnumerator C() {
				var length = Source.clip.length;

				IsStarted = true;
				IsPaused = false;
				yield return Wait.Seconds(length - Source.time);
				loopPointReached?.Invoke();
				if (!Source.loop) {
					IsStarted = false;
					yield break;
				}

				var wait = new WaitForSeconds(length);
				while (true) {
					yield return wait;
					loopPointReached?.Invoke();
				}
			}
			C().RunOn(this, ref _timerCoroutine);
		}

		public void Pause() {
			if (IsStarted && !IsPaused) {
				IsPaused = true;
				this.StopCoroutine(ref _timerCoroutine);
				Source.Pause();
			}
		}

		public void Stop() {
			if (IsStarted) {
				IsStarted = false;
				IsPaused = false;
				this.StopCoroutine(ref _timerCoroutine);
				Source.Stop();
				Source.time = 0;
			}
		}
	}
}
