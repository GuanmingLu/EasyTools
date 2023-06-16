using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyTools.Reflection {
	internal partial class Uncapsulator {
		internal Type Type => _type;
		internal BindingFlags BindingFlags => _defaultBindingFlags;
		internal IEnumerable<Type> AllTypes => GetTypeHierarchy(_type);
	}

	public class GettableValue {
		protected object _target;
		protected MemberInfo _member;
		public string Name => _member.Name;
		public string DisplayName => @$"{Name}{_member switch {
			FieldInfo => " { get; }",
			MethodInfo method => " ()",
			_ => ""
		}}";
		protected GettableValue() { }

		protected static bool CanCreate(MemberInfo member, Type getValueType) {
			var valueType = member switch {
				FieldInfo field => field.FieldType,
				PropertyInfo property when property.CanRead => property.PropertyType,
				MethodInfo method when method.GetParameters().Length == 0 => method.ReturnType,
				_ => null
			};
			return valueType != null && (getValueType?.IsAssignableFrom(valueType) ?? true);
		}

		internal static GettableValue Create(object target, MemberInfo member, Type getValueType = null)
			=> CanCreate(member, getValueType) ? new GettableValue() { _target = target, _member = member } : null;

		public object Get() => _member switch {
			FieldInfo field => field.GetValue(_target),
			PropertyInfo property => property.GetValue(_target),
			MethodInfo method => method.Invoke(_target, null),
			_ => default
		};
		public T Get<T>() => (T)Get();

		public (Type returnType, object returnValue) GetWithType() => _member switch {
			FieldInfo field => (field.FieldType, field.GetValue(_target)),
			PropertyInfo property => (property.PropertyType, property.GetValue(_target)),
			MethodInfo method => (method.ReturnType, method.Invoke(_target, null)),
			_ => (null, default)
		};
	}

	public class SettableValue {
		protected object _target;
		protected MemberInfo _member;
		public string Name => _member.Name;
		public string DisplayName => @$"{Name}{_member switch {
			PropertyInfo => " { set; }",
			MethodInfo method => $" ( {method.GetParameters()[0].ParameterType.Name} )",
			_ => ""
		}}";
		protected SettableValue() { }

		protected static Type GetMemberType(MemberInfo member) => member switch {
			FieldInfo field => field.FieldType,
			PropertyInfo property when property.CanWrite => property.PropertyType,
			MethodInfo method when method.GetParameters().Length == 1 => method.GetParameters()[0].ParameterType,
			_ => null
		};

		protected static bool CanCreate(MemberInfo member, Type setValueType) {
			var valueType = GetMemberType(member);
			return valueType != null && (setValueType == null || valueType.IsAssignableFrom(setValueType));
		}

		internal static SettableValue Create(object target, MemberInfo member, Type setValueType = null)
			=> CanCreate(member, setValueType) ? new SettableValue() { _target = target, _member = member } : null;

		public void Set(object value) {
			switch (_member) {
				case FieldInfo field:
					field.SetValue(_target, value);
					break;
				case PropertyInfo property:
					property.SetValue(_target, value);
					break;
				case MethodInfo method:
					method.Invoke(_target, new object[] { value });
					break;
			}
		}
	}

	public class Reflector {
		private Uncapsulator _uncap;
		public dynamic Dynamic => _uncap;
		private object _target => _uncap.Value;
		private Type _type => _uncap.Type;
		private BindingFlags _bindings => _uncap.BindingFlags;
		private IEnumerable<Type> _allTypes => _uncap.AllTypes;
		internal Reflector(Uncapsulator uncap) => _uncap = uncap;

		public IEnumerable<MemberInfo> GetMembers() => _allTypes.SelectMany(type => type.GetMembers(_bindings));
		private IEnumerable<MemberInfo> GetValueMembers(string memberName)
			=> _allTypes.SelectMany(type => type.GetMember(memberName, MemberTypes.Field | MemberTypes.Property | MemberTypes.Method, _bindings));

		#region 方法

		internal class MyInvokeMemberBinder : InvokeMemberBinder {
			public MyInvokeMemberBinder(string name) : base(name, false, new(0)) { }
			public List<Type> m_typeArguments = new();
			public Type[] TypeArguments = new Type[0];
			public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion) => throw new System.NotImplementedException();
			public override DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion) => throw new System.NotImplementedException();
		}

		public bool TryCall(string methodName, out object result, params object[] args) {
			if (_uncap.TryInvokeMember(new MyInvokeMemberBinder(methodName), args, out result, new(args.Length + 1))) {
				result = result is Uncapsulator uc ? uc.Value : result;
				return true;
			}
			return false;
		}

		#endregion

		#region 读取

		public IEnumerable<GettableValue> GetGettableValues(Type valueType = null)
			=> GetMembers().Select(member => GettableValue.Create(_target, member, valueType)).Where(v => v != null);
		public IEnumerable<GettableValue> GetGettableValues(string memberName, Type valueType = null)
			=> GetValueMembers(memberName).Select(member => GettableValue.Create(_target, member, valueType)).Where(v => v != null);
		public IEnumerable<GettableValue> GetGettableValues<T>() => GetGettableValues(typeof(T));
		public IEnumerable<GettableValue> GetGettableValues<T>(string memberName) => GetGettableValues(memberName, typeof(T));
		public bool TryGet<T>(string name, out T result) => GetGettableValues<T>(name).Select(v => v.Get<T>()).TryGetFirst(out result);

		#endregion

		#region 写入

		public IEnumerable<SettableValue> GetSettableValues(Type valueType = null)
			=> GetMembers().Select(member => SettableValue.Create(_target, member, valueType)).Where(v => v != null);
		public IEnumerable<SettableValue> GetSettableValues(string memberName, Type valueType = null)
			=> GetValueMembers(memberName).Select(member => SettableValue.Create(_target, member, valueType)).Where(v => v != null);
		public IEnumerable<SettableValue> GetSettableValues<T>() => GetSettableValues(typeof(T));
		public IEnumerable<SettableValue> GetSettableValues<T>(string memberName) => GetSettableValues(memberName, typeof(T));
		public bool TrySet<T>(string name, T value) => GetSettableValues<T>(name).TryGetFirst(out var setValue) && True(() => setValue.Set(value));

		#endregion

		private bool True(Action action) {
			action();
			return true;
		}
	}

	/// <summary>
	/// 反射相关的辅助工具 <br/>
	/// 注意：运行效率极低！请勿在运行环境下频繁调用！
	/// </summary>
	public static class Reflection {
		internal static dynamic Uncapsulate<T>(this T instance, bool useGlobalCache = false, bool publicMembersOnly = false)
			=> new Uncapsulator(null, new Uncapsulator.UncapsulatorOptions(useGlobalCache, publicMembersOnly), instance, callSiteType: typeof(T));
		internal static UncapsulatorException Wrap(this Exception ex, string msg = null) => new UncapsulatorException(msg ?? ex.Message, ex);

		/// <summary>
		/// 对任意对象进行反射操作 <br/>
		/// 注意：运行效率极低！请勿在运行环境下频繁调用！
		/// </summary>
		public static Reflector Reflect<T>(this T instance, bool useGlobalCache = false, bool publicMembersOnly = false)
			=> new(new(null, new(useGlobalCache, publicMembersOnly), instance, callSiteType: typeof(T)));

		/// <summary>
		/// 对类型进行反射操作 <br/>
		/// 注意：运行效率极低！请勿在运行环境下频繁调用！
		/// </summary>
		public static Reflector GetReflector(this Type type) => new(new(null, null, null, type));

		/// <summary>
		/// 对类型进行反射操作 <br/>
		/// 注意：运行效率极低！请勿在运行环境下频繁调用！
		/// </summary>
		public static Reflector GetReflector<T>() => typeof(T).GetReflector();

		/// <summary>
		/// 对类型进行反射操作 <br/>
		/// 注意：运行效率极低！请勿在运行环境下频繁调用！
		/// </summary>
		public static Reflector GetReflector(string fullTypeName, Assembly assembly) => assembly.GetType(fullTypeName, true).GetReflector();

		/// <summary>
		/// 对类型进行反射操作 <br/>
		/// 注意：运行效率极低！请勿在运行环境下频繁调用！
		/// </summary>
		public static Reflector GetReflector(string fullTypeName, string simpleAssemblyName) => GetReflector(fullTypeName, Assembly.Load(simpleAssemblyName));

		public static void ClearCache() => Uncapsulator.ClearStaticCache();
	}

}
