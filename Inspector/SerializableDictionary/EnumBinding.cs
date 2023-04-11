using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

	/// <summary>
	/// 绑定枚举和值
	/// </summary>
	[Serializable]
	public class EnumBinding<TKey, TValue> : SerializableDictionary<TKey, TValue> where TKey : Enum {
		protected override void ModifyDict() {
			HashSet<TKey> keys = new HashSet<TKey>(Keys);
			foreach (TKey key in Enum.GetValues(typeof(TKey))) {
				keys.Remove(key);
				if (!ContainsKey(key)) Add(key, default);
			}
			foreach (var otherKey in keys) {
				Remove(otherKey);
			}
		}
	}

	[Serializable] public class Storage<T> { public List<T> m_values = new List<T>(); }

	/// <summary>
	/// 绑定枚举和列表
	/// </summary>
	[Serializable]
	public class EnumListBinding<TKey, TList> : EnumBinding<TKey, Storage<TList>> where TKey : Enum {
		protected override Storage<TList> GetValue(Storage<TList> value) {
			if (value?.m_values != null) return value;
			else return new Storage<TList>();
		}

		new public List<TList> this[TKey key] {
			get => base[key].m_values;
			set => base[key].m_values = value;
		}

	}

}
