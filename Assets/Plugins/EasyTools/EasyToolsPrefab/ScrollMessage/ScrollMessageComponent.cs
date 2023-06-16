using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.InternalComponent {
	internal class ScrollMessageComponent : MonoBehaviour {
		[SerializeField] private GameObject _prefab;
		[SerializeField] private float _messageFadeOutTime = 1f;
		[SerializeField] private float _scrollTime = 0.5f;

		private float _height;
		private Vector2 _defaultPos;
		private RectTransform _rect;

		private void Awake() {
			_height = GetComponent<VerticalLayoutGroup>().spacing + _prefab.transform.ToRect().rect.height;
			_rect = transform as RectTransform;
			_defaultPos = _rect.anchoredPosition;
		}

		Coroutine _scrollCoroutine;
		internal void ShowMsg(string message, float duration = 3f) {
			if (message == null) return;

			Instantiate(_prefab, _rect).GetComponent<ScrollItem>().Show(message, _scrollTime, duration, _messageFadeOutTime);

			_rect.anchoredPosition -= new Vector2(0, _height);

			var startPos = _rect.anchoredPosition;
			EasyTween.Linear(_scrollTime, d => _rect.anchoredPosition = Vector2.Lerp(startPos, _defaultPos, d)).RunOn(this, ref _scrollCoroutine);
		}
	}
}
