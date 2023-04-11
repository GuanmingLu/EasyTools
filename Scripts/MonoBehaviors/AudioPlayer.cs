using System.Collections;
using System;
using UnityEngine;

namespace EasyTools {

	[RequireComponent(typeof(AudioSource))]
	public class AudioPlayer : MonoBehaviour {
		public AudioSource Source { get; private set; }
		private bool _playOnAwake;
		public bool IsStarted { get; private set; } = false;
		public bool IsPaused { get; private set; } = false;

		public event Action loopPointReached;

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
			_timer = C().Run(this);
		}

		public void Pause() {
			if (IsStarted && !IsPaused) {
				IsPaused = true;
				_timer?.Stop();
				Source.Pause();
			}
		}

		public void Stop() {
			if (IsStarted) {
				IsStarted = false;
				IsPaused = false;
				_timer?.Stop();
				Source.Stop();
				Source.time = 0;
			}
		}

		CoroutineHandler _timer;
	}
}
