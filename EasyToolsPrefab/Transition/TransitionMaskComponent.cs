using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.InternalComponent {
	internal class TransitionMaskComponent : MonoBehaviour {
		[SerializeField] internal Image _mask;
		[SerializeField] internal Text _text;
		[SerializeField] internal GameObject _loading;

		private void Awake() {
			_mask.enabled = false;
			_text.text = "";
		}
	}
}
