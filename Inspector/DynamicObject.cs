using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace EasyTools {

	[Serializable]
	public class DrawableObject : IDrawableObject {
		private Func<IEnumerable<(string name, Type type, object value)>> _getObj;
		public DrawableObject(Func<IEnumerable<(string name, Type type, object value)>> getObjMethod) {
			_getObj = getObjMethod;
		}
		IEnumerable<(string name, Type type, object value)> IDrawableObject.GetObj() => _getObj();
	}

	[Serializable]
	public class EditableObject : Dictionary<string, (Type objType, object value)>, ISerializationCallbackReceiver, IEditableObject {
		[SerializeField, HideInInspector] private List<string> _keys = new List<string>();
		[SerializeField, HideInInspector] private List<string> _types = new List<string>();
		[SerializeField, HideInInspector] private List<UObj> _UObjValues = new List<UObj>();
		[SerializeField, HideInInspector] private List<string> _othersValuesJson = new List<string>();
		void ISerializationCallbackReceiver.OnAfterDeserialize() {
			Clear();
			foreach (var (key, i) in _keys.GetIndex()) {
				var type = Type.GetType(_types[i]);
				if (typeof(UObj).IsAssignableFrom(type)) {
					this[key] = (type, _UObjValues[i]);
				}
				else {
					this[key] = (type, _othersValuesJson[i].FromJson(type));
				}
			}
		}
		void ISerializationCallbackReceiver.OnBeforeSerialize() {
			_keys.Clear();
			_types.Clear();
			_UObjValues.Clear();
			_othersValuesJson.Clear();
			foreach (var (key, (type, value)) in this) {
				_keys.Add(key);
				_types.Add(type.AssemblyQualifiedName);
				if (typeof(UObj).IsAssignableFrom(type)) {
					_UObjValues.Add((UObj)value);
					_othersValuesJson.Add(null);
				}
				else {
					_UObjValues.Add(null);
					_othersValuesJson.Add(value.ToJson());
				}
			}
		}
		IEnumerable<(string name, Type type, object value)> IDrawableObject.GetObj() {
			return this.Select(x => (x.Key, x.Value.objType, x.Value.value));
		}
		void IEditableObject.SetObj(string name, object obj) {
			var type = obj == null ? this[name].objType : obj.GetType();
			this[name] = (type, obj);
		}
	}

	[Serializable]
	public class DynamicObject : EditableObject {
		public void Add(string name, Type type, object obj) {
			this[name] = (type, obj);
		}
	}
}
