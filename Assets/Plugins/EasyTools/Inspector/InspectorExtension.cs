using System;
using UnityEngine;

namespace EasyTools.Inspector {
	[Serializable]
	public abstract class ValueHolder<T> {
		[SerializeField] private T m_value;
		protected ValueHolder(T value) => m_value = value;
		public T Value { get => m_value; set => m_value = value; }
		public static implicit operator T(ValueHolder<T> self) => self.m_value;
	}

	/// <summary>
	/// 在 Inspector 中显示一个按钮
	/// </summary>
	[Serializable]
	public class InspectorButton {
		[NonSerialized] public string text, methodName;

		/// <param name="nameof_method">要调用的方法名</param>
		/// <param name="text">按钮上显示的文字（留空则显示方法名）</param>
		public InspectorButton(string nameof_method, string text = null) {
			this.methodName = nameof_method;
			this.text = text ?? nameof_method;
		}
	}

	/// <summary>
	/// 在 Inspector 中显示一个只读值
	/// </summary>
	[Serializable]
	public class ReadOnlyValue {
		public string DisplayedName { get; private set; }
		public string ValueName { get; private set; }

		/// <param name="nameof_value">要显示的字段或属性的名称</param>
		/// <param name="displayedName">显示在 Inspector 上的标签（留空则显示字段或属性的名称）</param>
		public ReadOnlyValue(string nameof_value, string displayedName = null) {
			this.ValueName = nameof_value;
			this.DisplayedName = displayedName ?? nameof_value;
		}
	}

	/// <summary>
	/// 在 Inspector 中显示场景选择下拉框
	/// </summary>
	/// <remarks>
	/// 注意：该类储存场景名称，故场景文件被重命名时该引用会丢失
	/// </remarks>
	[Serializable]
	public class SceneName : ValueHolder<string> {
		/// <param name="defaultSceneName">场景名称默认值</param>
		public SceneName(string defaultSceneName = "SampleScene") : base(defaultSceneName) { }
		public static implicit operator SceneName(string self) => new SceneName(self);
	}

	/// <summary>
	/// 在 Inspector 中显示场景选择下拉框
	/// </summary>
	/// <remarks>
	/// 注意：该类储存场景的 buildIndex，故场景被重新排序时会产生错误引用
	/// </remarks>
	[Serializable]
	public class SceneIndex : ValueHolder<int> {
		/// <param name="sceneIndex">场景索引默认值</param>
		public SceneIndex(int sceneIndex = 0) : base(sceneIndex) { }
		public static implicit operator SceneIndex(int self) => new SceneIndex(self);
	}

	public interface IAnimatorParam {
		string AnimatorName { get; }
		AnimatorControllerParameterType? ParamType { get; }
	}

	/// <summary>
	/// 在 Inspector 中显示动画机的动画参数选择下拉框
	/// </summary>
	/// <remarks>
	/// 注意：该类储存参数名称，故参数被重命名时该引用会丢失
	/// </remarks>
	[Serializable]
	public class AnimatorParamName : ValueHolder<string>, IAnimatorParam {
		public string AnimatorName { get; private set; } = null;
		public AnimatorControllerParameterType? ParamType { get; private set; } = null;

		/// <param name="paramType">要选择的参数类型（留空表示所有类型）</param>
		/// <param name="nameof_animator">动画机的名称（留空将自动使用 <see cref="Component.GetComponent{T}"/> 获取）</param>
		public AnimatorParamName(AnimatorControllerParameterType? paramType = null, string nameof_animator = null) : base(default) {
			AnimatorName = nameof_animator;
			ParamType = paramType;
		}
	}

	/// <summary>
	/// 在 Inspector 中显示动画机的动画参数选择下拉框
	/// </summary>
	/// <remarks>
	/// 注意：该类储存参数的哈希值
	/// </remarks>
	[Serializable]
	public class AnimatorParamHash : ValueHolder<int>, IAnimatorParam {
		public string AnimatorName { get; private set; } = null;
		public AnimatorControllerParameterType? ParamType { get; private set; } = null;

		/// <param name="paramType">要选择的参数类型（留空表示所有类型）</param>
		/// <param name="nameof_animator">动画机的名称（留空将自动使用 <see cref="Component.GetComponent{T}"/> 获取）</param>
		public AnimatorParamHash(AnimatorControllerParameterType? paramType = null, string nameof_animator = null) : base(default) {
			AnimatorName = nameof_animator;
			ParamType = paramType;
		}
	}

	/// <summary>
	/// 在 Inspector 中选择一个资源文件，可获取其路径
	/// </summary>
	[Serializable]
	public class StreamingAssetFile : ValueHolder<string> {
		public StreamingAssetFile() : base(string.Empty) { }
	}
}
