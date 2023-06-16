using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EasyTools {

	public static class RangeExtension {

		public static IEnumerator<int> GetEnumerator(this Range range) {
			if (range.Start.IsFromEnd || range.End.IsFromEnd) {
				throw new NotSupportedException("Range with IsFromEnd is not supported");
			}
			for (int i = range.Start.Value; i < range.End.Value; i++) {
				yield return i;
			}
		}

		public static IEnumerable<int> Step(this Range range, int step) {
			if (range.Start.IsFromEnd || range.End.IsFromEnd) {
				throw new NotSupportedException("Range with IsFromEnd is not supported");
			}
			for (int i = range.Start.Value; i < range.End.Value; i += step) {
				yield return i;
			}
		}
	}
}
