using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

namespace EasyTools.InternalComponent {

	internal class VideoPlayerEventTrigger : MonoBehaviour {
		[SerializeField] private UnityEvent m_onVideoEnd;

		private void Awake() {
			GetComponent<VideoPlayer>().loopPointReached += _ => m_onVideoEnd.Invoke();
		}
	}
}
