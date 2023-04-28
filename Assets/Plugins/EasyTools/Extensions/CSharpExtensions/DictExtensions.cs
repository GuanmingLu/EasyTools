using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyTools {

	public static class DictExtensions {
		public static int RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> source, Func<TKey, TValue, bool> predicate) {
			var keysToRemove = new HashSet<TKey>();
			foreach (var (key, value) in source) {
				if (predicate(key, value)) keysToRemove.Add(key);
			}
			foreach (var key in keysToRemove) {
				source.Remove(key);
			}
			return keysToRemove.Count;
		}

		public static TValue GetOrInit<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TValue> initFunc) {
			if (!source.ContainsKey(key)) {
				source[key] = initFunc == null ? default : initFunc();
			}
			return source[key];
		}
	}

	public class AutoInitDict<TKey, TValue> : Dictionary<TKey, TValue> {
		private Func<TKey, TValue> _initFunc;
		public AutoInitDict(Func<TKey, TValue> initFunc) {
			_initFunc = initFunc;
		}

		public new TValue this[TKey key] {
			set => base[key] = value;
			get => this.GetValueOrDefault(key, _initFunc == null ? default : _initFunc.Invoke(key));
		}
	}
}
