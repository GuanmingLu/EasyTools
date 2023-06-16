using System.Collections.Generic;
using System;
using static System.Math;
using System.Linq;

namespace EasyTools {

	public static class EasyMath {
		public static int Clamp(int value, int min, int max) => Max(Min(value, max), min);
		public static float Clamp(float value, float min, float max) => Max(Min(value, max), min);
		public static double Clamp(double value, double min, double max) => Max(Min(value, max), min);

		private static Random rand = new Random();

		public static int RandInt(int minInclusive, int maxExclusive) => rand.Next(minInclusive, maxExclusive);
		public static double RandDouble(float minInclusive, float maxInclusive) => rand.NextDouble() * (maxInclusive - minInclusive) + minInclusive;
		public static float RandFloat(float minInclusive, float maxInclusive) => (float)rand.NextDouble() * (maxInclusive - minInclusive) + minInclusive;

		public static float Sqr(this float value) => value * value;
	}
}
