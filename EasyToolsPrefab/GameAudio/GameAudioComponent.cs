using System.Collections;
using UnityEngine;

namespace EasyTools.InternalComponent {
	internal class GameAudioComponent : MonoBehaviour {
		[SerializeField] private AudioSource _sfxSource, _bgmSource;

		private void Awake() {
			_bgmSource.playOnAwake = false;
			_bgmSource.loop = true;
		}

		internal void PlaySFX(AudioClip sfx, float volumeScale) {
			if (sfx != null) _sfxSource.PlayOneShot(sfx, volumeScale);
		}

		Coroutine _fadeCoroutine;
		internal void FadeBGM(AudioClip targetBGM, float fadeTime, float volume) {
			IEnumerator C() {
				// 淡出
				yield return EasyTween.Linear(fadeTime, t => _bgmSource.volume = t, _bgmSource.volume, 0);

				// 切换
				_bgmSource.Stop();
				_bgmSource.clip = targetBGM;

				// 淡入
				if (targetBGM != null) {
					_bgmSource.Play();
					yield return EasyTween.Linear(fadeTime, t => _bgmSource.volume = t, 0, volume);
				}
				else {
					_bgmSource.volume = volume;
				}
			}
			if (targetBGM != _bgmSource.clip) C().RunOn(this, ref _fadeCoroutine);
		}

		internal void FadeVolume(float targetVolume, float fadeTime) {
			IEnumerator C() {
				yield return EasyTween.Linear(fadeTime, t => _bgmSource.volume = t, _bgmSource.volume, targetVolume);
			}
			C().RunOn(this, ref _fadeCoroutine);
		}
	}
}
