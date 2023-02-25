using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.InternalComponent {

	[RequireComponent(typeof(Button))]
	internal class ButtonSound : MonoBehaviour {
		[SerializeField] private AudioClip[] m_clickSounds;

		private void Awake() {
			GetComponent<Button>().onClick.AddListener(() => GameAudio.PlaySFX(m_clickSounds.ChooseRandom()));
		}
	}
}
