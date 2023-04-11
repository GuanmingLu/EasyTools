using System.Collections;
using UnityEngine;

namespace EasyTools.InternalComponent {
	internal class GameAudioComponent : MonoBehaviour {
		internal static GameAudioComponent Instance { get; private set; }

		[SerializeField] private AudioSource _sfxSource, _bgmSource;
		private bool _fading = false;

		private void Awake() {
			Instance = this;

			_bgmSource.playOnAwake = false;
			_bgmSource.loop = true;
		}

		internal void PlaySFX(AudioClip sfx, float volumeScale) {
			if (sfx != null) _sfxSource.PlayOneShot(sfx, volumeScale);
		}

		internal void FadeBGM(AudioClip targetBGM, float fadeTime, float volume) {
			if (!_fading && targetBGM != _bgmSource.clip) BGM_Fade(targetBGM, fadeTime, volume).Run(this);
		}
		IEnumerator BGM_Fade(AudioClip targetBGM, float fadeTime, float volume) {
			_fading = true;

			// 淡出
			var v = _bgmSource.volume;
			yield return EasyTween.Linear(fadeTime, d => _bgmSource.volume = Mathf.Lerp(v, 0, d));

			// 切换
			_bgmSource.Stop();
			_bgmSource.clip = targetBGM;

			// 淡入
			if (targetBGM != null) {
				_bgmSource.Play();
				yield return EasyTween.Linear(fadeTime, d => _bgmSource.volume = Mathf.Lerp(0, volume, d));
			}
			else {
				_bgmSource.volume = volume;
			}

			_fading = false;
		}
	}
}
