using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {
	public class EasyTask : IEnumerable {
		private List<object> _seq = new List<object>();

		public void Add(object obj) => _seq.Add(obj);
		public void Add(Action obj) => _seq.Add(obj);

		public IEnumerator ToCoroutine() {
			foreach (var item in _seq) {
				if (item is Action action) action?.Invoke();
				else yield return item;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => ToCoroutine();

		public CoroutineHandler Run(MonoBehaviour mono) => ToCoroutine().Run(mono);
		public CoroutineHandler Run() => ToCoroutine().Run();
	}
}
