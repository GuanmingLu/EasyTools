using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools {
	partial class EasyToolsGameObject {
		[Header("GameSettingsPanel")]
		[SerializeField] internal CanvasGroup m_panel;
		[SerializeField] private Text m_resolutionText;
		[SerializeField] internal CanvasGroup m_about;

		private void InitGameSettingsPanel() {
			ChangeResolution();
			SetFullScreen(true);
		}

		private static Vector2 _nativeResolution = new(Screen.width, Screen.height);
		private Vector2[] _resolutions;
		private int _resolutionIndex = -1;
		private void ChangeResolution() {
			var r = (_resolutions ??= new Vector2[] {
				_nativeResolution,
				_nativeResolution / 1.2f,
				_nativeResolution / 1.5f,
				_nativeResolution / 2f,
			}).ChooseLoop(++_resolutionIndex);
			Vector2Int resolution = new(Mathf.RoundToInt(r.x), Mathf.RoundToInt(r.y));
			Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreenMode);
			m_resolutionText.text = $"{resolution.x}x{resolution.y}";
		}
		private void SetFullScreen(bool isOn) => Screen.fullScreenMode = isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

		public void UIEvent_SetFullScreen(bool isOn) => SetFullScreen(isOn);
		public void UIEvent_ChangeResolution() => ChangeResolution();
		public void UIEvent_ChangeBGMVolume(float value) => GameAudio.GlobalVolume = value;
		public void UIEvent_ChangeSFXVolume(float value) => GameAudio.SfxVolume = value;
		public void UIEvent_ShowAbout() => GameSettingsPanel.ShowAbout();
		public void UIEvent_HideAbout() => GameSettingsPanel.HideAbout();
		public void UIEvent_OpenPanel() => GameSettingsPanel.Open();
		public void UIEvent_ClosePanel() => GameSettingsPanel.Close();
		public void UIEvent_Exit() => MainMenuOrExit();
	}

	public static class GameSettingsPanel {
		private static EasyToolsGameObject Instance => EasyToolsGameObject.Instance;

		private static Easing _showPanel = new Easing(Instance, 0, 1, EasingType.Linear).SetDuration(0.2f)
			.OnValueChanged(t => Instance.m_panel.alpha = t)
			.OnStartForward(() => Instance.m_panel.gameObject.SetActive(true))
			.OnFinishBackward(() => Instance.m_panel.gameObject.SetActive(false));

		public static void Open() => _showPanel.RunForward();
		public static void Close() => _showPanel.RunBackward();

		private static Easing _showAbout = new Easing(Instance, 0, 1, EasingType.Linear).SetDuration(0.2f)
			.OnValueChanged(t => Instance.m_about.alpha = t)
			.OnStartForward(() => Instance.m_about.gameObject.SetActive(true))
			.OnFinishBackward(() => Instance.m_about.gameObject.SetActive(false));

		public static void ShowAbout() => _showAbout.RunForward();
		public static void HideAbout() => _showAbout.RunBackward();
	}
}
