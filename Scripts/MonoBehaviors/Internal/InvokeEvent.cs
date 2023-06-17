using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.InternalComponent {

	internal class InvokeEvent : MonoBehaviour {
		private enum InvokeTime {
			Awake, OnEnable, Start
		}
		[SerializeField] private InvokeTime m_invokeTime = InvokeTime.Start;
		[SerializeField] private float m_delay;
		[SerializeField] private UnityEngine.Events.UnityEvent m_event;

		private void Awake() {
			if (m_invokeTime == InvokeTime.Awake) m_event.Invoke();
		}

		private void OnEnable() {
			if (m_invokeTime == InvokeTime.OnEnable) m_event.Invoke();
		}

		private void Start() {
			if (m_invokeTime == InvokeTime.Start) m_event.Invoke();
		}
	}
}
