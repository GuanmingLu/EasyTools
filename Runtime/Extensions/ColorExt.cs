using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools {

	/// <summary>
	/// 使用 32 位无符号整型（uint / System.UInt32）表示的颜色 <br/>
	/// 整型格式为 0xRRGGBBAA，Alpha 通道不可以省略
	/// </summary>
	public static class ColorU {
		#region 颜色常量

		public const uint
		red = 0xFF0000FF,
		green = 0x008000FF,
		blue = 0x0000FFFF,

		cyan = 0x00FFFFFF,
		magenta = 0xFF00FFFF,
		yellow = 0xFFFF00FF,

		clear = 0x00000000,
		black = 0x000000FF,
		white = 0xFFFFFFFF,
		grey = 0x7F7F7FFF,
		gray = 0x7F7F7FFF,

		pink = 0xF7C5D4FF,
		orange = 0xFFA500FF,
		indigo = 0x4B0082FF,
		violet = 0xEE82EEFF;

		#endregion

		public static uint ToUInt(this Color32 color) => BitConverter.ToUInt32(new byte[] { color.a, color.b, color.g, color.r }, 0);
		public static uint ToUInt(this Color color) => ToUInt((Color32)color);
		public static string ToHex(this Color32 color) => "#" + BitConverter.ToString(new byte[] { color.a, color.b, color.g, color.r }).Replace("-", "");
		public static string ToHex(this Color color) => ToHex((Color32)color);

		/// <summary> 将 32 位无符号整数（如 "0x40A070FF"）转换为对应的颜色 </summary>
		/// <remarks> 注意 Alpha 值不可以省略（如 "0x43B244" 会得到错误的颜色） </remarks>
		public static Color32 Parse(uint color) {
			var bytes = BitConverter.GetBytes(color);
			return new Color32(bytes[3], bytes[2], bytes[1], bytes[0]);
		}

		/// <summary> 将形如 "#FF9900" 或 "#EE8800FF" 的字符串转换为对应的颜色 </summary>
		public static Color32 Parse(string hex) {
			if (string.IsNullOrWhiteSpace(hex) || hex[0] != '#') throw new FormatException($"{hex} 不是有效的颜色：字符串必须以 # 开头");

			hex = (hex + "FF")[1..9];   // 补齐 Alpha 值

			return Parse(Convert.ToUInt32(hex, 16));
		}
	}
}
