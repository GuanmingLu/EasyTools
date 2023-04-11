using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EasyTools.Reflection {

	/// <summary>
	/// 反射相关的辅助工具 <br/>
	/// 注意：运行效率极低！请勿在运行环境下频繁调用！
	/// </summary>
	public static class ReflectionUtils {

		public class GettableValue<T> {
			public Type ValueType { get; private set; }
			public MemberInfo Member { get; private set; }

			internal GettableValue(object target, MemberInfo member) {
				Member = member;
				if (Member is FieldInfo field) {
					ValueType = field.FieldType;
					_getValue = () => (T)field.GetValue(target);
				}
				else if (Member is PropertyInfo property && property.CanRead) {
					ValueType = property.PropertyType;
					_getValue = () => (T)property.GetValue(target);
				}
				else if (Member is MethodInfo method && method.GetParameters().Length == 0) {
					ValueType = method.ReturnType;
					_getValue = () => (T)method.Invoke(target, null);
				}
				else {
					throw new Exception("该成员不能获取值");
				}
				if (!typeof(T).IsAssignableFrom(ValueType)) throw new TargetException($"该成员的类型是 {ValueType}，无法将其转换为 {typeof(T)} 类型");
			}

			private Func<T> _getValue;
			public T Get() => _getValue();
		}
		public class SettableValue<T> {
			public Type ValueType { get; private set; }
			public MemberInfo Member { get; private set; }

			internal SettableValue(object target, MemberInfo member) {
				Member = member;
				if (Member is FieldInfo field) {
					ValueType = field.FieldType;
					_setValue = value => field.SetValue(target, value);
				}
				else if (Member is PropertyInfo property && property.CanWrite) {
					ValueType = property.PropertyType;
					_setValue = value => property.SetValue(target, value);
				}
				else if (Member is MethodInfo method && method.GetParameters().Length == 1) {
					ValueType = method.GetParameters()[0].ParameterType;
					_setValue = value => method.Invoke(target, new object[] { value });
				}
				else {
					throw new Exception("该成员不能设置值");
				}
				if (!ValueType.IsAssignableFrom(typeof(T))) throw new TargetException($"该成员的类型是 {ValueType} ，无法将 {typeof(T)} 类型赋值给它");
			}

			private Action<T> _setValue;
			public void Set(T value) => _setValue(value);
		}

		public const BindingFlags Bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		/// <summary>
		/// 注意：运行效率极低！请勿在运行环境下频繁调用！
		/// </summary>
		public class Reflector {
			private Type _type;
			private bool _includeBase;
			internal Reflector(Type type, bool includeBase) {
				_type = type;
				_includeBase = includeBase;
			}

			public IEnumerable<Type> GetAllClasses() {
				if (_includeBase) {
					var type = _type;
					while (type != null) {
						yield return type;
						foreach (var t in type.GetInterfaces()) {
							yield return t;
						}
						type = type.BaseType;
					}
				}
				else yield return _type;
			}
			public IEnumerable<T> GetMembers<T>() where T : MemberInfo {
				foreach (var type in GetAllClasses()) {
					foreach (var m in type.GetMembers(Bindings)) {
						if (m is T member) yield return member;
					}
				}
			}
			public IEnumerable<T> GetMembers<T>(string memberName) where T : MemberInfo => GetMembers<T>().Where(member => member.Name == memberName);
			public IEnumerable<MemberInfo> GetMembers() => GetMembers<MemberInfo>();
			public IEnumerable<MemberInfo> GetMembers(string memberName) => GetMembers().Where(member => member.Name == memberName);

			public object CallStatic(string methodName, params object[] args) {
				if (GetMembers<MethodInfo>().TryGetFirst(out var func, m => {
					var parameters = m.GetParameters();
					if (m.Name != methodName || !m.IsStatic || parameters.Length != args.Length) return false;
					for (int i = 0; i < parameters.Length; i++) {
						if (!parameters[i].ParameterType.IsAssignableFrom(args[i].GetType())) return false;
					}
					return true;
				})) {
					return func.Invoke(null, args);
				}
				else {
					throw new Exception($"找不到 {_type.Name}.{methodName}({string.Join(", ", args.Select(a => a.GetType().Name))}) 方法");
				}
			}
		}
		public static Reflector ReflectAsType(this Type type, bool includeBase = true) => new Reflector(type, includeBase);

		/// <summary>
		/// 注意：运行效率极低！请勿在运行环境下频繁调用！
		/// </summary>
		public class ObjectReflector : Reflector {
			private object _target;
			internal ObjectReflector(object target, bool includeBase) : base(target.GetType(), includeBase) => _target = target;

			#region 方法

			public bool TryCall(string methodName, out object result, params object[] args) {
				if (GetMembers<MethodInfo>(methodName).TryGetFirst(out var method)) {
					result = method.Invoke(_target, args);
					return true;
				}
				result = null;
				return false;
			}
			public bool TryCall(string methodName, params object[] args) => TryCall(methodName, out _, args);

			#endregion

			#region 读取

			public IEnumerable<GettableValue<T>> GetGettableValues<T>()
				=> GetMembers().Where(member => member.IsGettableTo(typeof(T))).Select(member => new GettableValue<T>(_target, member));

			public IEnumerable<GettableValue<T>> GetGettableValues<T>(string memberName)
				=> GetGettableValues<T>().Where(member => member.Member.Name == memberName);

			#endregion

			#region 写入

			public IEnumerable<SettableValue<T>> GetSettableValues<T>()
				=> GetMembers().Where(member => member.IsSettableFrom(typeof(T))).Select(member => new SettableValue<T>(_target, member));

			public IEnumerable<SettableValue<T>> GetSettableValues<T>(string memberName)
				=> GetSettableValues<T>().Where(member => member.Member.Name == memberName);

			#endregion
		}
		public static ObjectReflector Reflect(this object target, bool includeBase = true) => new ObjectReflector(target, includeBase);

		public static bool IsGettableTo(this MemberInfo member, Type type)
			=> (member is FieldInfo fieldInfo && type.IsAssignableFrom(fieldInfo.FieldType))
			|| (member is PropertyInfo property && property.CanRead && type.IsAssignableFrom(property.PropertyType))
			|| (member is MethodInfo method && method.GetParameters().Length == 0 && type.IsAssignableFrom(method.ReturnType));

		public static bool IsSettableFrom(this MemberInfo member, Type type)
			=> (member is FieldInfo fieldInfo && fieldInfo.FieldType.IsAssignableFrom(type))
			|| (member is PropertyInfo property && property.CanWrite && property.PropertyType.IsAssignableFrom(type))
			|| (member is MethodInfo method && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType.IsAssignableFrom(type));
	}
}
