using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

	public class BgmPlayer : MonoBehaviour {
		[SerializeField] private AudioClip _bgm;
		[SerializeField] private float _delay;
		[SerializeField] private float _volume = 1f;
		private enum FadeType {
			Awake, OnEnable, Start
		}
		[SerializeField] private FadeType _bgmFadeType = FadeType.Start;

		private void Awake() {
			if (_bgmFadeType == FadeType.Awake) Fade().Run(this);
		}

		private void OnEnable() {
			if (_bgmFadeType == FadeType.OnEnable) Fade().Run(this);
		}

		private void Start() {
			if (_bgmFadeType == FadeType.Start) Fade().Run(this);
		}

		IEnumerator Fade() {
			yield return new WaitForSeconds(_delay);
			GameAudio.FadeBGM(bgm: _bgm, volume: _volume);
		}
	}
}
