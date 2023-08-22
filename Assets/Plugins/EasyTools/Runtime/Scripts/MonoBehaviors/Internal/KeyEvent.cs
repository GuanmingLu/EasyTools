using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EasyTools.InternalComponent {

	internal class KeyEvent : MonoBehaviour {
		[SerializeField] private KeyCode[] m_keyCodes;
		[SerializeField] private UnityEvent m_onKeyDown;
		[SerializeField] private UnityEvent m_onKeyUp;

		private KeyCode? _currentDownKey;

		private void Update() {
			if (_currentDownKey is KeyCode downKey) {
				if (Input.GetKeyUp(downKey)) {
					_currentDownKey = null;
					m_onKeyUp.Invoke();
				}
			}
			else {
				foreach (var key in m_keyCodes) {
					if (Input.GetKeyDown(key)) {
						_currentDownKey = key;
						m_onKeyDown.Invoke();
					}
				}
			}
		}
	}
}
