using UnityEngine;
using EasyTools.InternalComponent;
using System.Collections;

namespace EasyTools {
	partial class EasyToolsGameObject {
		[Header("GameAudio")]
		[SerializeField] internal AudioSource m_sfxSource;
		[SerializeField] internal AudioSource m_bgmSource;
	}

	public static class GameAudio {
		private static EasyToolsGameObject Instance => EasyToolsGameObject.Instance;
		private static AudioSource BgmSource => Instance.m_bgmSource;
		private static AudioSource SfxSource => Instance.m_sfxSource;

		private static float _volume = 1f;
		public static float GlobalVolume {
			get => _volume;
			set {
				_volume = value;
				FadeVolume(value, 0.2f);
			}
		}

		public static float SfxVolume {
			get => SfxSource.volume;
			set => SfxSource.volume = value;
		}

		private static Coroutine _audioCoroutine;

		private static void FadeVolume(float targetVolume, float fadeTime) {
			EasyTween.Linear(fadeTime, t => BgmSource.volume = t, BgmSource.volume, targetVolume)
			.RunOn(Instance, ref _audioCoroutine);
		}

		/// <summary>
		/// 播放音效（可同时播放多个，不会覆盖）
		/// </summary>
		public static void PlaySFX(AudioClip sfx, float volumeScale = 1f) {
			if (sfx != null) SfxSource.PlayOneShot(sfx, volumeScale);
		}

		/// <summary>
		/// BGM淡出淡入切换
		/// </summary>
		public static void FadeBGM(AudioClip targetBGM, float fadeTime = 0.2f, float volume = 1f) {
			volume *= GlobalVolume;
			IEnumerator C() {
				// 淡出
				yield return EasyTween.Linear(fadeTime, t => BgmSource.volume = t, BgmSource.volume, 0);

				// 切换
				BgmSource.Stop();
				BgmSource.clip = targetBGM;

				// 淡入
				if (targetBGM != null) {
					BgmSource.Play();
					yield return EasyTween.Linear(fadeTime, t => BgmSource.volume = t, 0, volume);
				}
				else {
					BgmSource.volume = volume;
				}
			}
			if (targetBGM != BgmSource.clip) C().RunOn(Instance, ref _audioCoroutine);
		}
	}
}
