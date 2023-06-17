using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.InternalComponent {
	/// <summary>
	/// 挂载于要接收 OnTriggerExit 事件的 GameObject 上，提供更可靠的检测 <br/>
	/// 当正在碰撞的触发器被禁用时会发送 OnTriggerExit 消息
	/// </summary>
	internal class TriggerHelper : MonoBehaviour {
		private HashSet<Collider> _colliders = new HashSet<Collider>();
		private HashSet<Collider> _exitedColliders = new HashSet<Collider>();
		private void OnTriggerEnter(Collider other) {
			_colliders.Add(other);
		}
		private void OnTriggerExit(Collider other) {
			_colliders.Remove(other);
		}
		private void Update() {
			_exitedColliders.Clear();
			foreach (var c in _colliders) {
				if (c == null || c.gameObject == null || !c.gameObject.activeInHierarchy || !c.enabled) _exitedColliders.Add(c);
			}
			foreach (var c in _exitedColliders) {
				SendMessage("OnTriggerExit", c);
			}
		}
	}
}
