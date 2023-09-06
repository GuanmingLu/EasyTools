using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyTools.Inspector {
	/// <summary>
	/// 在 Inspector 中绘制一条水平线
	/// </summary>
	public class HorizontalLineAttribute : PropertyAttribute {
		public float height = 2;
		public Color color = Color.gray;
		public HorizontalLineAttribute() { }
		public HorizontalLineAttribute(float height = 2) => this.height = height;
		public HorizontalLineAttribute(float height = 2, string hexColor = "#7F7F7F") : this(height) => this.color = ColorU.Parse(hexColor);
		public HorizontalLineAttribute(float height = 2, uint hexColor = 0x7F7F7FFF) : this(height) => this.color = ColorU.Parse(hexColor);
	}
}
