using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.UI {

	[RequireComponent(typeof(CanvasRenderer))]
	public class EmptyUI : Graphic {
		protected override void Reset() {
			color = Color.clear;
		}
	}
}
