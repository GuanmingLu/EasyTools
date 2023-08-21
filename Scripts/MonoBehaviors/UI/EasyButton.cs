using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace EasyTools.UI {

	public class EasyButton : EasySelectable, IPointerClickHandler, ISubmitHandler {
		[SerializeField] private UnityEvent m_OnClick = new();
		public UnityEvent OnClick => m_OnClick;
		[SerializeField] private float m_doubleClickTime = 0.4f;
		[SerializeField] public UnityEvent m_OnDoubleClick = new();
		public UnityEvent OnDoubleClick => m_OnDoubleClick;
		[SerializeField] private float m_longPressTime = 0.4f;
		[SerializeField] private UnityEvent m_OnLongPress = new();
		public UnityEvent OnLongPress => m_OnLongPress;


		public new bool IsPressed { get; private set; }
		public bool WasPressedThisFrame { get; private set; }
		public bool WasReleasedThisFrame { get; private set; }
		private void LateUpdate() {
			WasPressedThisFrame = false;
			WasReleasedThisFrame = false;
		}

		Coroutine _longPressTimer;
		private bool _longPressTriggered;

		Coroutine _doubleClickTimer;

		private void StartPress() {
			if (!IsActive() || !IsInteractable()) return;

			IsPressed = true;
			WasPressedThisFrame = true;

			_longPressTriggered = false;
			Wait.Seconds(m_longPressTime).Yield(() => {
				OnLongPress?.Invoke();
				_longPressTriggered = true;
			}).RunOn(this, ref _longPressTimer);
		}

		private void EndPress() {
			IsPressed = false;
			WasReleasedThisFrame = true;
			this.StopCoroutine(ref _longPressTimer);
		}

		private void HandleClick() {
			if (!IsActive() || !IsInteractable()) return;

			if (_longPressTriggered) return;    // 如果已经被判定为长按了，就不触发点击

			if (_doubleClickTimer == null) {
				UISystemProfilerApi.AddMarker("Button.onClick", this);
				OnClick?.Invoke();
				Wait.Seconds(m_doubleClickTime).Yield(() => _doubleClickTimer = null).RunOn(this, ref _doubleClickTimer);
			}
			else {
				UISystemProfilerApi.AddMarker("Button.onDoubleClick", this);
				OnDoubleClick?.Invoke();
				this.StopCoroutine(ref _doubleClickTimer);
			}
		}

		Coroutine _submitTransition;
		private void HandleSubmit() {
			if (!IsActive() || !IsInteractable()) return;

			UISystemProfilerApi.AddMarker("Button.onClick", this);
			OnClick?.Invoke();

			DoStateTransition(SelectionState.Pressed, false);
			Wait.Seconds(colors.fadeDuration).Yield(() => DoStateTransition(currentSelectionState, false)).RunOn(this, ref _submitTransition);
		}

		#region 接口实现

		public virtual void OnPointerClick(PointerEventData eventData) => HandleClick();

		public override void OnPointerDown(PointerEventData eventData) {
			base.OnPointerDown(eventData);
			StartPress();
		}
		public override void OnPointerUp(PointerEventData eventData) {
			base.OnPointerUp(eventData);
			EndPress();
		}
		public override void OnPointerExit(PointerEventData eventData) {
			base.OnPointerExit(eventData);
			EndPress();
		}

		public virtual void OnSubmit(BaseEventData eventData) => HandleSubmit();

		public void UIEvent_SetPressing(bool pressing) {
			if (pressing) StartPress();
			else EndPress();
		}

		#endregion

	}
}
