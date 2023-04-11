using System;
using System.Linq;
using System.Collections.Generic;
using static EasyTools.EasyMath;

namespace EasyTools {

	public static class ListExtensions {
		public static IEnumerable<(T item, int index)> GetIndex<T>(this IEnumerable<T> source, int startIndex = 0)
			=> source.Zip(Enumerable.Range(startIndex, source.Count()), (item, index) => (item, index));

		#region First & Last

		public static bool TryGetFirst<T>(this IEnumerable<T> source, out T first) {
			foreach (var item in source) {
				first = item;
				return true;
			}
			first = default;
			return false;
		}

		public static bool TryGetFirst<T>(this IEnumerable<T> source, out T first, Func<T, bool> predicate)
			=> source.Where(predicate).TryGetFirst(out first);

		public static T FirstOrInit<T>(this IEnumerable<T> source, Func<T> initFunc)
			=> source.TryGetFirst(out var first) ? first : initFunc == null ? default : initFunc();

		public static T FirstOrInit<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T> initFunc)
			=> source.TryGetFirst(out var first, predicate) ? first : initFunc == null ? default : initFunc();

		public static bool TryGetLast<T>(this IEnumerable<T> source, out T last) {
			foreach (var item in source.Reverse()) {
				last = item;
				return true;
			}
			last = default;
			return false;
		}

		public static bool TryGetLast<T>(this IEnumerable<T> source, out T last, Func<T, bool> predicate) {
			foreach (var item in source.Reverse()) {
				if (predicate(item)) {
					last = item;
					return true;
				}
			}
			last = default;
			return false;
		}

		public static T LastOrInit<T>(this IEnumerable<T> source, Func<T> initFunc)
			=> source.TryGetLast(out var last) ? last : initFunc == null ? default : initFunc();

		public static T LastOrInit<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T> initFunc)
			=> source.TryGetLast(out var last, predicate) ? last : initFunc == null ? default : initFunc();

		#endregion

		#region ForEach

		/// <param name="action"> 对集合中各元素的操作 </param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (var item in source) action(item);
		}

		/// <param name="actionWithIndex"> 对集合中各元素的操作，参数为 (item, index) </param>
		/// <param name="startIndex"> index 的起始值 </param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> actionWithIndex, int startIndex = 0) {
			int index = startIndex;
			foreach (var item in source) actionWithIndex(item, index++);
		}

		/// <param name="action"> 对集合中各元素的操作，返回 true 时跳出循环 </param>
		/// <returns> 跳出循环时返回 true，完全遍历时返回 false </returns>
		public static bool ForEach<T>(this IEnumerable<T> source, Func<T, bool> action) {
			foreach (var item in source) {
				if (action(item)) return true;
			}
			return false;
		}

		/// <param name="actionWithIndex"> 对集合中各元素的操作，参数为 (item, index)，返回 true 时跳出循环 </param>
		/// <param name="startIndex"> index 的起始值 </param>
		/// <returns> 跳出循环时返回 true，完全遍历时返回 false </returns>
		public static bool ForEach<T>(this IEnumerable<T> source, Func<T, int, bool> actionWithIndex, int startIndex = 0) {
			int index = startIndex;
			foreach (var item in source) {
				if (actionWithIndex(item, index++)) return true;
			}
			return false;
		}

		#endregion

		/// <summary> 相当于 source[index % source.Count()] </summary>
		public static T ChooseLoop<T>(this IEnumerable<T> source, int index) => source.ElementAt(index % source.Count());

		/// <summary> 相当于将 source[index] 中的 index 自动限制在 [0, source.Count() - 1] </summary>
		public static T ChooseClamp<T>(this IEnumerable<T> source, int index) => source.ElementAt(Clamp(index, 0, source.Count() - 1));

		/// <summary> 随机选取列表中的一个元素 </summary>
		public static T ChooseRandom<T>(this IEnumerable<T> source) => source.ElementAt(RandInt(0, source.Count()));
	}
}
