using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace EasyTools.Animation {

	public class SpriteAnimation : MonoBehaviour {
		[SerializeField] private Sprite[] m_sprites;
		[SerializeField] private float m_frameRate;
		[SerializeField] private bool _playOnStart;
		[SerializeField] private float _playDelay;

		public UnityEvent m_loopPointReached;

		private int _index;
		private int Index {
			get => _index;
			set {
				_index = value;
				if (_index >= m_sprites.Length) {
					_index = 0;
					m_loopPointReached?.Invoke();
				}
			}
		}

		private void Start() {
			if (_playOnStart) Play();
		}

		Coroutine _coroutine;
		public void Play() {
			if (m_sprites.Length == 0) return;

			Index = 0;
			var wait = new WaitForSeconds(1 / m_frameRate);
			IEnumerator C() {
				yield return Wait.Seconds(_playDelay);
				if (TryGetComponent<SpriteRenderer>(out var sr))
					while (true) {
						sr.sprite = m_sprites[Index++];
						yield return wait;
					}
				else if (TryGetComponent<Image>(out var img))
					while (true) {
						img.sprite = m_sprites[Index++];
						yield return wait;
					}
				else if (TryGetComponent<RawImage>(out var rawImg))
					while (true) {
						rawImg.texture = m_sprites[Index++].texture;
						yield return wait;
					}
			}
			C().RunOn(this, ref _coroutine);
		}

		public void Stop() => this.StopCoroutine(ref _coroutine);
	}
}
