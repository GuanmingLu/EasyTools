using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.InternalComponent {

	internal class BgmPlayer : MonoBehaviour {
		[SerializeField] private AudioClip _bgm;
		[SerializeField] private float _delay;
		[SerializeField] private float _volume = 1f;
		private enum FadeType {
			Awake, OnEnable, Start
		}
		[SerializeField] private FadeType _bgmFadeType = FadeType.Start;

		private void Awake() {
			if (_bgmFadeType == FadeType.Awake) Fade();
		}

		private void OnEnable() {
			if (_bgmFadeType == FadeType.OnEnable) Fade();
		}

		private void Start() {
			if (_bgmFadeType == FadeType.Start) Fade();
		}

		Coroutine _fadeCoroutine;
		void Fade() {
			Wait.Seconds(_delay).Yield(() => GameAudio.FadeBGM(_bgm, volume: _volume))
			.RunOn(this, ref _fadeCoroutine);
		}
	}
}
