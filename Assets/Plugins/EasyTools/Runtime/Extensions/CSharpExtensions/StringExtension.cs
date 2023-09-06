using System;
using System.Globalization;
using System.Text.RegularExpressions;

public static class StringExtension {

	public static bool TryMatch(this Regex regex, string input, out Match match) {
		match = regex.Match(input);
		return match.Success;
	}

	private static Regex _unicodeReg = new(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);
	public static string DecodeUnicode(this string str) {
		return _unicodeReg.Replace(str, m => {
			return short.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var c)
				? ((char)c).ToString()
				: m.Value;
		});
	}
}
