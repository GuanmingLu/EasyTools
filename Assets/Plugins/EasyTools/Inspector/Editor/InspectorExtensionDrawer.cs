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
			if (string.IsNullOrEmpty(PropValue.methodName)) {
				Utils.Warning(position, $"{property.name}: 按钮方法名为空");
			}
			else if (GUI.Button(position, PropValue.text)) {
				if (!Mono.Reflect().TryCall(PropValue.methodName)) {
					Debug.LogError($"按钮方法 {PropValue.methodName} 调用失败");
				}
			}
		}
	}

	[CustomPropertyDrawer(typeof(ReadOnlyValue))]
	public class ReadOnlyValueDrawer : PropDrawerBase<ReadOnlyValue> {
		protected override void OnGUI(Rect position) {
			if (string.IsNullOrEmpty(PropValue.ValueName)) {
				Utils.Warning(position, "未设置只读值名称");
			}
			else {
				if (Mono.Reflect().GetGettableValues<object>(PropValue.ValueName).TryGetFirst(out var member)) {
					using (new EditorGUI.DisabledScope(true))
						Utils.ValueField(position, PropValue.DisplayedName, member.Get(), member.ValueType);
				}
				else {
					Utils.Warning(position, $"未找到值 {PropValue.ValueName}");
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
			var value = property.FindPropertyRelative("m_value");
			EditorGUI.BeginProperty(position, label, property);

			if (value.propertyType == SerializedPropertyType.String) {
				var index = Mathf.Clamp(Array.IndexOf(allSceneNames, value.stringValue), 0, allSceneNames.Length - 1);
				var newIndex = EditorGUI.Popup(position, label, index, displayNames);
				if (newIndex != index) {
					value.stringValue = allSceneNames[newIndex];
				}
			}
			else if (value.propertyType == SerializedPropertyType.Integer) {
				var index = Mathf.Clamp(value.intValue, 0, allSceneNames.Length - 1);
				var newIndex = EditorGUI.Popup(position, label, index, displayNames);
				if (newIndex != index) {
					value.intValue = newIndex;
				}
			}

			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer(typeof(AnimatorParamName))]
	[CustomPropertyDrawer(typeof(AnimatorParamHash))]
	public class AnimatorParamDrawer : PropDrawerBase<IAnimatorParam> {
		private AnimatorControllerParameter[] GetParams() {   // 筛选出符合条件的动画参数
			Animator animator = null;
			if (string.IsNullOrEmpty(PropValue.AnimatorName)) {
				if (!Mono.TryGetComponent<Animator>(out animator)) {
					throw new Exception($"未设置 Animator Name，且在此 GameObject 上未找到 Animator");
				}
			}
			else {
				if (Mono.Reflect().GetGettableValues<Animator>(PropValue.AnimatorName).TryGetFirst(out var member)) {
					animator = member.Get();
					if (animator == null) {
						throw new Exception($"{PropValue.AnimatorName} 的值为 null");
					}
				}
				else {
					throw new Exception($"在脚本中未找到名为 {PropValue.AnimatorName} 的 Animator");
				}
			}
			var controller = (AnimatorController)animator.runtimeAnimatorController;
			if (controller == null) {
				throw new Exception($"{PropValue.AnimatorName} 没有设置 Controller");
			}
			var animParams = controller.parameters.Where(parameter => PropValue.ParamType == null || parameter.type == PropValue.ParamType).ToArray();
			if (animParams.Length == 0) {
				throw new Exception($"Animator 中没有 {PropValue.ParamType} 类型的动画参数");
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
			var valueProp = property.FindPropertyRelative("m_value");
			EditorGUI.BeginProperty(position, label, property);

			if (valueProp.propertyType == SerializedPropertyType.Integer) {
				int selectedIndex = Array.FindIndex(animParams, param => param.nameHash == valueProp.intValue);
				selectedIndex = selectedIndex < 0 ? 0 : selectedIndex;

				int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, displayOptions);
				int newValue = animParams[newIndex].nameHash;

				if (valueProp.intValue != newValue) {
					valueProp.intValue = newValue;
				}
			}
			else if (valueProp.propertyType == SerializedPropertyType.String) {
				int selectedIndex = Array.FindIndex(animParams, param => param.name == valueProp.stringValue);
				selectedIndex = selectedIndex < 0 ? 0 : selectedIndex;

				int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, displayOptions);
				string newValue = animParams[newIndex].name;

				if (!valueProp.stringValue.Equals(newValue, System.StringComparison.Ordinal)) {
					valueProp.stringValue = newValue;
				}
			}

			EditorGUI.EndProperty();
		}
	}
}
