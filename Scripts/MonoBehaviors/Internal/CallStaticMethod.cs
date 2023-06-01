using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using EasyTools.Inspector;

namespace EasyTools.InternalComponent {
	[Serializable]
	public class StaticCall {
		public string className = "";
		public string methodName = "";
	}

	public class CallStaticMethod : MonoBehaviour {
		public const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod;

		[SerializeField] private StaticCall[] m_callList;

		public void UIEvent_Call() {
			foreach (var call in m_callList) {
				Type.GetType(call.className)?.InvokeMember(call.methodName, flags, Type.DefaultBinder, null, null);
			}
		}

#if UNITY_EDITOR

		[SerializeField] private InspectorButton m_callBtn = new(nameof(UIEvent_Call), "Call");

#endif
	}
}
