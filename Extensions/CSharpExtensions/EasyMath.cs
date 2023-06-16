using System.Collections.Generic;
using System;
using static System.Math;
using System.Linq;

namespace EasyTools {

	public static class EasyMath {
		public static int ClampMin(this int value, int min) => Max(value, min);
		public static int ClampMax(this int value, int max) => Min(value, max);
		public static int Clamp(this int value, int min, int max) => Max(Min(value, max), min);
		public static float Clamp(this float value, float min, float max) => Max(Min(value, max), min);
		public static double Clamp(this double value, double min, double max) => Max(Min(value, max), min);

		private static Random rand = new Random();

		public static int RandInt(int minInclusive, int maxExclusive) => rand.Next(minInclusive, maxExclusive);
		public static double RandDouble(float minInclusive, float maxInclusive) => rand.NextDouble() * (maxInclusive - minInclusive) + minInclusive;
		public static float RandFloat(float minInclusive, float maxInclusive) => (float)rand.NextDouble() * (maxInclusive - minInclusive) + minInclusive;

		public static float Sqr(this float value) => value * value;
	}
}
