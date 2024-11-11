using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.UI {

	[RequireComponent(typeof(EasyButton))]
	internal class EasyButtonSound : MonoBehaviour {
		[SerializeField] private AudioClip[] m_clickSounds;

		private void Awake() {
			GetComponent<EasyButton>().OnClick.AddListener(() => GameAudio.PlaySFX(m_clickSounds.ChooseRandom()));
		}
	}
}
