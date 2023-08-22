using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

namespace EasyTools {

	public struct TMP_SingleTag {
		public string name;
		public Dictionary<string, string> parameters;
		public string MainParam => parameters[name];
		public int index;

		public readonly bool GetBool(string key, bool defaultValue = false)
			=> parameters.TryGetValue(key, out var str) && bool.TryParse(str, out var value) ? value : defaultValue;
		public readonly float GetFloat(string key, float defaultValue = 0)
			=> parameters.TryGetValue(key, out var str) && float.TryParse(str, out var value) ? value : defaultValue;
		public readonly int GetInt(string key, int defaultValue = 0)
			=> parameters.TryGetValue(key, out var str) && int.TryParse(str, out var value) ? value : defaultValue;
		public readonly Vector2 GetV2(string key, Vector2 defaultValue) {
			if (parameters.TryGetValue(key, out var str)) {
				var arr = str.Split(',');
				if (arr.Length == 2 && float.TryParse(arr[0], out var x) && float.TryParse(arr[1], out var y)) {
					return new Vector2(x, y);
				}
			}
			return defaultValue;
		}
		public readonly Vector3 GetV3(string key, Vector3 defaultValue) {
			if (parameters.TryGetValue(key, out var str)) {
				var arr = str.Split(',');
				if (arr.Length == 3 && float.TryParse(arr[0], out var x) && float.TryParse(arr[1], out var y) && float.TryParse(arr[2], out var z)) {
					return new Vector3(x, y, z);
				}
			}
			return defaultValue;
		}
	}


	public static class TMP_TagExtension {
		private static string ReplaceRegex(this string input, string regex, string replacement)
			=> string.IsNullOrWhiteSpace(regex) ? input : Regex.Replace(input, regex, replacement, RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private const string SingleTagLink = "cTag";
		private const string RangeTagStartLink = "startTag";
		private const string RangeTagEndLink = "endTag";

		public static string PreProcessDialog(string dialog, string singleTags = null, string rangeTags = null) {
			return dialog
			.ReplaceRegex(singleTags != null ? $"<((?:{singleTags}).*?)>" : null, $"<link={SingleTagLink} $1></link>")
			.ReplaceRegex(rangeTags != null ? $"<((?:{rangeTags}).*?)>" : null, $"<link={RangeTagStartLink} $1></link>")
			.ReplaceRegex(rangeTags != null ? $"</({rangeTags}).*?>" : null, $"<link={RangeTagEndLink} $1></link>");
		}

		private static TMP_SingleTag? GetSingleTag(TMP_LinkInfo link) {
			var arr = link.GetLinkID().Split(' ');
			if (arr.Length > 1 && arr[0] == SingleTagLink) {
				var parameters = arr.Skip(1).Select(s => {
					var index = s.IndexOf('=');
					return index >= 0 ? new KeyValuePair<string, string>(s[..index], s[(index + 1)..]) : new KeyValuePair<string, string>(s, string.Empty);
				});
				return new TMP_SingleTag {
					name = parameters.First().Key,
					parameters = parameters.ToDictionary(),
					index = link.linkTextfirstCharacterIndex
				};
			}
			else {
				return null;
			}
		}

		public static IEnumerable<TMP_SingleTag> GetSingleTagsOfIndex(this TMP_Text text, int index) => text.GetSingleTags().Where(t => t.index == index);

		public static IEnumerable<TMP_SingleTag> GetSingleTags(this TMP_Text text) => text.textInfo.linkInfo.Take(text.textInfo.linkCount).SelectNotNull(GetSingleTag);
	}
}
