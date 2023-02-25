using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace EasyTools.Events {
	public enum TargetRequirement {
		Any,
		NotNull,
		ActiveGO,
		Enabled,
		ActiveAndEnabled,
	}
	public class EasyEventBase<TDelegate> where TDelegate: Delegate {
		private Dictionary<TDelegate, (UObj _target, TargetRequirement requirement)> _eventMap = new();
		public void AddListener(TDelegate callback) => _eventMap.Add(callback, (null, TargetRequirement.Any));
		public void AddListener(UObj target, TDelegate callback, TargetRequirement requirement = TargetRequirement.NotNull)
			=> _eventMap.Add(callback, (target, requirement));
		public void AddListener(GameObject target, TDelegate callback, TargetRequirement requirement = TargetRequirement.ActiveGO)
			=> _eventMap.Add(callback, (target, requirement));
		public void AddListener(Component target, TDelegate callback, TargetRequirement requirement = TargetRequirement.ActiveGO)
			=> _eventMap.Add(callback, (target, requirement));
		public void AddListener(Behaviour target, TDelegate callback, TargetRequirement requirement = TargetRequirement.ActiveAndEnabled)
			=> _eventMap.Add(callback, (target, requirement));
		protected IEnumerable<TDelegate> GetInvokeList() {
			foreach (var (action, (target, req)) in _eventMap) {
				if (req == TargetRequirement.Any) yield return action;
				else if (target == null) _eventMap.Remove(action);
				else if (req switch {
					TargetRequirement.NotNull => true,
					TargetRequirement.ActiveGO => (target is GameObject go && go.activeSelf) || (target is Component c && c.gameObject.activeSelf),
					TargetRequirement.Enabled => target is Behaviour b && b.enabled,
					TargetRequirement.ActiveAndEnabled => target is Behaviour b && b.isActiveAndEnabled,
					_ => false,
				}) yield return action;
			}
		}
	}
	public class EasyFuncBase<TFunc> : EasyEventBase<TFunc> where TFunc: Delegate {
		new public IEnumerable<TFunc> GetInvokeList() => base.GetInvokeList();
	}
}
