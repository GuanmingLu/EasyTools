using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyTools {

	public static class EasyConvert {
		private const string DefaultCharacterSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		public static string ToBase62(byte[] data) => ToBase62(data, DefaultCharacterSet);
		public static string ToBase62(byte[] data, string characterSet) {
			var arr = Array.ConvertAll(data, t => (int)t);
			var converted = BaseConvert(arr, 256, 62);
			var builder = new StringBuilder();
			foreach (var t in converted) {
				builder.Append(characterSet[t]);
			}
			return builder.ToString();
		}

		public static byte[] FromBase62(string base62str) => FromBase62(base62str, DefaultCharacterSet);
		public static byte[] FromBase62(string base62str, string characterSet) {
			if (base62str.Any(c => !characterSet.Contains(c))) throw new ArgumentException("发现不在字符集中的字符", nameof(base62str));
			var arr = Array.ConvertAll(base62str.ToCharArray(), characterSet.IndexOf);
			var converted = BaseConvert(arr, 62, 256);
			return Array.ConvertAll(converted, Convert.ToByte);
		}

		private static int[] BaseConvert(int[] source, int sourceBase, int targetBase) {
			var result = new List<int>();
			var leadingZeroCount = Math.Min(source.TakeWhile(x => x == 0).Count(), source.Length - 1);
			int count;
			while ((count = source.Length) > 0) {
				var quotient = new List<int>();
				var remainder = 0;
				for (var i = 0; i != count; i++) {
					var accumulator = source[i] + remainder * sourceBase;
					var digit = accumulator / targetBase;
					remainder = accumulator % targetBase;
					if (quotient.Count > 0 || digit > 0) {
						quotient.Add(digit);
					}
				}
				result.Insert(0, remainder);
				source = quotient.ToArray();
			}
			result.InsertRange(0, Enumerable.Repeat(0, leadingZeroCount));
			return result.ToArray();
		}
	}
}
