using System;
using System.Linq;
using System.Collections.Generic;
using static EasyTools.EasyMath;

namespace EasyTools {

	public static class ListExtensions {
		public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source, int startIndex = 0)
			=> source.Select(item => (item, startIndex++));

		public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
			foreach (var (item, index) in source.WithIndex()) {
				if (predicate(item)) return index;
			}
			return -1;
		}

		public static IEnumerable<(int i, int j)> GetIndex<T>(this T[,] source) {
			for (int i = 0; i < source.GetLength(0); i++) {
				for (int j = 0; j < source.GetLength(1); j++) {
					yield return (i, j);
				}
			}
		}

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

		/// <param name="action"> 对集合中各元素的操作 </param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (var item in source) action(item);
		}

		/// <summary> 相当于 source[index % source.Count()] </summary>
		public static T ChooseLoop<T>(this IEnumerable<T> source, int index) => source.ElementAt(index % source.Count());

		/// <summary> 相当于将 source[index] 中的 index 自动限制在 [0, source.Count() - 1] </summary>
		public static T ChooseClamp<T>(this IEnumerable<T> source, int index) => source.ElementAt(index.Clamp(0, source.Count() - 1));

		/// <summary> 随机选取列表中的一个元素 </summary>
		public static T ChooseRandom<T>(this IEnumerable<T> source) => source.ElementAt(RandInt(0, source.Count()));


		public static void Shuffle<T>(this List<T> source) {
			for (int i = 0; i < source.Count; i++) {
				int j = RandInt(i, source.Count);
				(source[i], source[j]) = (source[j], source[i]);
			}
		}
		public static void FillCapacity<T>(this List<T> source, T value) {
			while (source.Count < source.Capacity) source.Add(value);
		}
	}


	public class AutoInitDict<TKey, TValue> : Dictionary<TKey, TValue> {
		public new TValue this[TKey key] {
			set => base[key] = value;
			get => this.GetValueOrDefault(key);
		}
	}
}