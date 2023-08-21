using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EasyTools;
using EasyTools.Inspector;
using System.IO;

public class Test : MonoBehaviour {
	[SerializeField] private InspectorButton _button = new(nameof(TestFunc));
	[SerializeField] private ScriptableObject _instance;

	public void TestFunc() {
		var assetPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(_instance));
		Debug.Log(Directory.GetCurrentDirectory());
		Debug.Log(assetPath);
		Debug.Log(Path.GetFullPath("Packages"));
		Debug.Log(Path.GetFullPath("Packages/com"));
		Debug.Log(Path.GetFullPath("Packages/com.cysharp.unitask"));
		Debug.Log(Path.GetFullPath("Packages/com.unity.textmeshpro"));
		Debug.Log(Path.GetFullPath(assetPath));
	}
}
