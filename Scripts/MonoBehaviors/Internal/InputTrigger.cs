using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EasyTools.InternalComponent {

	internal class InputTrigger : MonoBehaviour {
		[SerializeField] private KeyCode m_keyCode;
		[SerializeField] private UnityEvent m_onKeyDown;

		private void Update() {
			if (Input.GetKeyDown(m_keyCode)) m_onKeyDown.Invoke();
		}
	}
}
