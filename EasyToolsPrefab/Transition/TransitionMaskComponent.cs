using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.InternalComponent {
	internal class TransitionMaskComponent : MonoBehaviour {
		internal static TransitionMaskComponent Instance { get; private set; }

		[SerializeField] internal Image _mask;
		[SerializeField] internal Text _text;

		private void Awake() {
			Instance = this;

			_mask.enabled = false;
			_text.text = "";
		}
	}
}
