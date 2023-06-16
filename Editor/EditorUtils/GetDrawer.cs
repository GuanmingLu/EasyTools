using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyTools.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {

	public static partial class Utils {

		private static bool IsPropertyDrawerOf(this Type drawer, Type targetType) {
			return drawer.GetCustomAttributes<CustomPropertyDrawer>().Any(attribute => {
				Type drawerTargetType = attribute.Reflect().Dynamic.m_Type;
				// TODO 使用该 CustomPropertyDrawer 的 useForChildren 字段判断是否绘制派生类
				return drawerTargetType == targetType;
			});
		}

		private static Dictionary<Type, PropDrawerBase> _propDrawerDict = new();
		private static bool TryGetPropDrawer(Type targetType, out PropDrawerBase drawer) {
			drawer = _propDrawerDict.GetOrInit(
				targetType,
				() => TypeCache.GetTypesDerivedFrom<PropDrawerBase>().TryGetFirst(out var drawerType, type => type.IsPropertyDrawerOf(targetType))
					? (PropDrawerBase)Activator.CreateInstance(drawerType) : null);
			return drawer != null;
		}

		private static Dictionary<Type, PropertyDrawer> _drawerDict = new();
		private static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer, bool setFieldInfo, FieldInfo fieldInfo, bool setAttribute, Attribute attribute) {
			drawer = _drawerDict.GetOrInit(
				targetType,
				() => TypeCache.GetTypesDerivedFrom<PropertyDrawer>().TryGetFirst(out var drawerType, type => type.IsPropertyDrawerOf(targetType))
					? (PropertyDrawer)Activator.CreateInstance(drawerType) : null
			);
			if (drawer != null) {
				if (setFieldInfo) drawer.Reflect().Dynamic.m_FieldInfo = fieldInfo;
				if (setAttribute) drawer.Reflect().Dynamic.m_Attribute = attribute;
				return true;
			}
			return false;
		}
		public static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer)
			=> TryGetDrawer(targetType, out drawer, false, null, false, null);
		public static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer, FieldInfo fieldInfo)
			=> TryGetDrawer(targetType, out drawer, true, fieldInfo, false, null);
		public static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer, Attribute attribute)
			=> TryGetDrawer(targetType, out drawer, false, null, true, attribute);
		public static bool TryGetDrawer(Type targetType, out PropertyDrawer drawer, FieldInfo fieldInfo, Attribute attribute)
			=> TryGetDrawer(targetType, out drawer, true, fieldInfo, true, attribute);
	}
}
