using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

	/// <summary>
	/// 可序列化字典
	/// </summary>
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
		[SerializeField] private TKey[] m_keys;
		[SerializeField] private TValue[] m_values;

		public void OnAfterDeserialize() {
			if (m_keys != null && m_values != null && m_keys.Length == m_values.Length) {
				Clear();
				int n = m_keys.Length;
				for (int i = 0; i < n; ++i) {
					this[m_keys[i]] = GetValue(m_values[i]);
				}
				m_keys = null;
				m_values = null;

				ModifyDict();
			}
		}

		public void OnBeforeSerialize() {
			ModifyDict();

			m_keys = new TKey[Count];
			m_values = new TValue[Count];

			int i = 0;
			foreach (var kvp in this) {
				m_keys[i] = kvp.Key;
				m_values[i] = GetValue(kvp.Value);
				++i;
			}
		}

		protected virtual TValue GetValue(TValue value) => value;
		protected virtual void ModifyDict() { }
	}

}
