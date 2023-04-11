using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyTools.Settings {

	public class UseAssetSettings : ResourceSingleton<UseAssetSettings> {
		public const BindingFlags BINDINGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		public static UseAssetSettings GetInstance() => Instance;

		[Serializable]
		public class UseAssetField {
			public string typeAssemblyQualifiedName;
			public string fieldName;
			public string fieldTypeAssemblyQualifiedName;
			public UnityEngine.Object uObjValue;
			public string otherValueJson;
		}
		[SerializeField] public List<UseAssetField> useAssetFields = new List<UseAssetField>();


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			var settings = GetInstance().useAssetFields;
			foreach (var field in settings) {
				var type = Type.GetType(field.typeAssemblyQualifiedName);
				if (type == null) continue;
				var f = type.GetField(field.fieldName, BINDINGS);
				if (f == null) continue;
				if (f.FieldType != Type.GetType(field.fieldTypeAssemblyQualifiedName)) continue;

				if (typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType)) {
					f.SetValue(null, field.uObjValue);
				}
				else {
					f.SetValue(null, field.otherValueJson.FromJson(f.FieldType));
				}
			}
		}
	}
}
