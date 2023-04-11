using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using EasyTools.Inspector;

namespace EasyTools {

	public class VideoPlayerPreview : MonoBehaviour {
		[SerializeField] private InspectorButton m_playBtn = new InspectorButton(nameof(Play), "播放");
		private void Play() {
			if (TryGetComponent<VideoPlayer>(out var c)) c.Play();
		}

		[SerializeField] private InspectorButton m_pauseBtn = new InspectorButton(nameof(Pause), "暂停");
		private void Pause() {
			if (TryGetComponent<VideoPlayer>(out var c)) c.Pause();
		}

		[SerializeField] private InspectorButton m_stopBtn = new InspectorButton(nameof(Stop), "停止");
		private void Stop() {
			if (TryGetComponent<VideoPlayer>(out var c)) c.Stop();
		}
	}
}
