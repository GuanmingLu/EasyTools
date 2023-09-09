using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.UI {

	[RequireComponent(typeof(CanvasRenderer))]
	public class EmptyUI : Graphic {
#if UNITY_EDITOR
		protected override void Reset() {
			color = Color.clear;
			base.Reset();
		}
#endif
	}
}
