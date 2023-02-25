using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.InternalComponent {

	[RequireComponent(typeof(Button))]
	internal class ButtonKeyInput : MonoBehaviour {
		[SerializeField] private KeyCode m_keyCode;

		private void Update() {
			if (Input.GetKeyDown(m_keyCode)) GetComponent<Button>().onClick.Invoke();
		}
	}
}
