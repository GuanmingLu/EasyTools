using System;
using System.Linq;
using System.Collections.Generic;
using static EasyTools.EasyMath;
using UnityEngine.UIElements;

namespace EasyTools {

	public static class ArrayExtensions {
		public static T[] Insert<T>(this T[] source, T value, int index) {
			var result = new T[source.Length + 1];
			if (index > 0) Array.Copy(source, result, index);
			result[index] = value;
			if (index < source.Length) Array.Copy(source, index, result, index + 1, source.Length - index);
			return result;
		}
		public static T[] Insert<T>(this T[] source, T[] values, int index) {
			var result = new T[source.Length + values.Length];
			if (index > 0) Array.Copy(source, result, index);
			Array.Copy(values, 0, result, index, values.Length);
			if (index < source.Length) Array.Copy(source, index, result, index + values.Length, source.Length - index);
			return result;
		}
	}
}
