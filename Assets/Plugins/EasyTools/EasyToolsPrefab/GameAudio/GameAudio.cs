using EasyTools.InternalComponent;
using UnityEngine;

namespace EasyTools {

	public static class GameAudio {
		private static GameAudioComponent Instance => GameAudioComponent.Instance;

		/// <summary>
		/// 播放音效（可同时播放多个，不会覆盖）
		/// </summary>
		public static void PlaySFX(AudioClip sfx, float volumeScale = 1f) => Instance.PlaySFX(sfx, volumeScale);

		/// <summary>
		/// BGM淡出淡入切换
		/// </summary>
		public static void FadeBGM(AudioClip bgm, float fadeTime = 0.2f, float volume = 1f) => Instance.FadeBGM(bgm, fadeTime, volume);
	}
}
