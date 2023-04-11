using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

	public interface IDrawableObject {
		// Dictionary<string, (Type objType, object value)> GetObj();
		IEnumerable<(string name, Type type, object value)> GetObj();
	}
	public interface IEditableObject : IDrawableObject {
		void SetObj(string name, object obj);
	}
	public interface IResizableObject : IEditableObject {
		void Add(string name, Type type, object obj);
		bool Remove(string name);
	}
}
