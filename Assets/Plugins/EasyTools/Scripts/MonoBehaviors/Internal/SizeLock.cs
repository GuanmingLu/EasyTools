using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.InternalComponent {

	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	internal class SizeLock : MonoBehaviour {
#if UNITY_EDITOR
		[SerializeField] private float m_width;
		[SerializeField] private float m_ratio;

		private RectTransform _rectTransform;
		private RectTransform RT => _rectTransform ??= GetComponent<RectTransform>();

		private void OnEnable() {
			m_width = RT.rect.width;
			m_ratio = RT.rect.height / m_width;
		}

		private void Update() {
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_width);
			RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_width * m_ratio);
		}
#endif
	}
}
