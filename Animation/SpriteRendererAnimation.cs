using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.Animation {

	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteRendererAnimation : MonoBehaviour {
		[SerializeField] private Sprite[] _sprites;
		[SerializeField] private float _frameRate;
		[SerializeField] private bool _playOnStart;
		[SerializeField] private bool _loop;

		private void Start() {
			if (_playOnStart) Play();
		}

		Coroutine _coroutine;
		public void Play() {
			var sr = GetComponent<SpriteRenderer>();
			float interval = 1 / _frameRate;
			int index = 0;
			IEnumerator C() {
				do {
					sr.sprite = _sprites.ChooseLoop(index++);
					yield return Wait.Seconds(interval);
				} while (_loop);
			}
			C().RunOn(this, ref _coroutine);
		}

		public void Stop() => this.StopCoroutine(ref _coroutine);
	}
}
