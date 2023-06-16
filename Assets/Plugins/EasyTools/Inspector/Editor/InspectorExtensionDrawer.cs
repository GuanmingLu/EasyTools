using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using EasyTools.Reflection;
using System.Collections.Generic;
using UnityEditor.Animations;
using EasyTools.Inspector;

namespace EasyTools.Editor {

	[CustomPropertyDrawer(typeof(InspectorButton))]
	public class InspectorButtonDrawer : PropDrawerBase<InspectorButton> {
		protected override void OnGUI(Rect position) {
			if (string.IsNullOrEmpty(propValue.methodName)) {
				Utils.Warning(position, $"{label.text}: 按钮方法名为空");
			}
			else if (GUI.Button(position, propValue.text)) {
				if (!owner.Reflect().TryCall(propValue.methodName, out _)) {
					Debug.LogError($"按钮方法 {propValue.methodName} 调用失败");
				}
			}
		}
	}

	[CustomPropertyDrawer(typeof(ReadOnlyValue))]
	public class ReadOnlyValueDrawer : PropDrawerBase<ReadOnlyValue> {
		protected override void OnGUI(Rect position) {
			if (string.IsNullOrEmpty(propValue.ValueName)) {
				Utils.Warning(position, "未设置只读值名称");
			}
			else {
				if (owner.Reflect().GetGettableValues(propValue.ValueName).TryGetFirst(out var member)) {
					var (type, value) = member.GetWithType();
					using (new EditorGUI.DisabledScope(true))
						Utils.ValueField(position, propValue.DisplayedName, value, type);
				}
				else {
					Utils.Warning(position, $"未找到值 {propValue.ValueName}");
				}
			}
		}
	}

	[CustomPropertyDrawer(typeof(SceneName))]
	[CustomPropertyDrawer(typeof(SceneIndex))]
	public class SceneNameIndexDrawer : PropDrawerBase<object> {
		protected override void OnGUI(Rect position) {
			var allSceneNames = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => Regex.Match(scene.path, @".+\/(.+)\.unity").Groups[1].Value).ToArray();
			if (allSceneNames.Length == 0) {
				Utils.Warning(position, "没有可用的场景");
				return;
			}

			var displayNames = allSceneNames.Select((scene, index) => new GUIContent($"{scene} ({index})")).ToArray();

			if (propValue is SceneName name) {
				var selectedIndex = Array.IndexOf(allSceneNames, name.Value).ClampMin(0);
				if (useProp) label = EditorGUI.BeginProperty(position, label, property);
				EditorGUI.BeginChangeCheck();
				var newIndex = EditorGUI.Popup(position, label, selectedIndex, displayNames);
				if (EditorGUI.EndChangeCheck()) {
					if (useProp) property.FindPropertyRelative("m_value").stringValue = allSceneNames[newIndex];
					else {
						Utils.ChangedObject(owner, $"Change SceneName prop value: {label.text} = {allSceneNames[newIndex]}");
						name.Value = allSceneNames[newIndex];
					}
				}
				if (useProp) EditorGUI.EndProperty();
			}
			else if (propValue is SceneIndex indexValue) {
				var selectedIndex = indexValue.Value.ClampMin(0);
				if (useProp) label = EditorGUI.BeginProperty(position, label, property);
				EditorGUI.BeginChangeCheck();
				var newIndex = EditorGUI.Popup(position, label, selectedIndex, displayNames);
				if (EditorGUI.EndChangeCheck()) {
					if (useProp) property.FindPropertyRelative("m_value").intValue = newIndex;
					else {
						Utils.ChangedObject(owner, $"Change SceneIndex prop value: {label.text} = {allSceneNames[newIndex]}");
						indexValue.Value = newIndex;
					}
				}
				if (useProp) EditorGUI.EndProperty();
			}
		}
	}

	[CustomPropertyDrawer(typeof(AnimatorParamName))]
	[CustomPropertyDrawer(typeof(AnimatorParamHash))]
	public class AnimatorParamDrawer : PropDrawerBase<IAnimatorParam> {
		private AnimatorControllerParameter[] GetParams() {   // 筛选出符合条件的动画参数
			Animator animator = null;
			if (string.IsNullOrEmpty(propValue.AnimatorName)) {
				if (!((Component)owner).TryGetComponent<Animator>(out animator)) {
					throw new Exception($"未设置 Animator Name，且在此 GameObject 上未找到 Animator");
				}
			}
			else {
				if (!owner.Reflect().TryGet(propValue.AnimatorName, out animator))
					throw new Exception($"在脚本中未找到名为 {propValue.AnimatorName} 的 Animator");
				if (animator == null)
					throw new Exception($"{propValue.AnimatorName} 的值为 null");
			}
			var controller = (AnimatorController)animator.runtimeAnimatorController;
			if (controller == null) {
				throw new Exception($"{propValue.AnimatorName} 没有设置 Controller");
			}
			var animParams = controller.parameters.Where(parameter => propValue.ParamType == null || parameter.type == propValue.ParamType).ToArray();
			if (animParams.Length == 0) {
				throw new Exception($"Animator 中没有 {propValue.ParamType} 类型的动画参数");
			}
			return animParams;
		}

		protected override void OnGUI(Rect position) {
			AnimatorControllerParameter[] animParams;
			try {
				animParams = GetParams();
			}
			catch (Exception e) {
				Utils.Warning(position, e.Message);
				return;
			}

			var displayOptions = animParams.Select(param => param.name).ToArray();

			if (propValue is AnimatorParamHash paramHash) {
				int selectedIndex = Array.FindIndex(animParams, p => p.nameHash == paramHash.Value).ClampMin(0);
				if (useProp) label = EditorGUI.BeginProperty(position, label, property);
				EditorGUI.BeginChangeCheck();
				var newValue = animParams[EditorGUI.Popup(position, label.text, selectedIndex, displayOptions)].nameHash;
				if (EditorGUI.EndChangeCheck()) {
					if (useProp) property.FindPropertyRelative("m_value").intValue = newValue;
					else {
						Utils.ChangedObject(owner, $"Change AnimatorParamHash prop value: {label.text} = {newValue}");
						paramHash.Value = newValue;
					}
				}
				if (useProp) EditorGUI.EndProperty();
			}
			else if (propValue is AnimatorParamName paramName) {
				int selectedIndex = Array.FindIndex(animParams, p => p.name == paramName.Value).ClampMin(0);
				if (useProp) label = EditorGUI.BeginProperty(position, label, property);
				EditorGUI.BeginChangeCheck();
				var newName = animParams[EditorGUI.Popup(position, label.text, selectedIndex, displayOptions)].name;
				if (EditorGUI.EndChangeCheck()) {
					if (useProp) property.FindPropertyRelative("m_value").stringValue = newName;
					else {
						Utils.ChangedObject(owner, $"Change AnimatorParamName prop value: {label.text} = {newName}");
						paramName.Value = newName;
					}
				}
				if (useProp) EditorGUI.EndProperty();
			}
		}
	}
}
