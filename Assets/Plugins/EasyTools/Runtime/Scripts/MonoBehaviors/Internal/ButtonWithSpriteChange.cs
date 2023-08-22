using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.InternalComponent {

	/// <summary>
	/// 该按钮在 Transition 为 ColorTint 时，会同时使用 SpriteSwap 中的配置
	/// </summary>
	internal class ButtonWithSpriteChange : Button {
		protected override void DoStateTransition(SelectionState state, bool instant) {
			base.DoStateTransition(state, instant);
			if (image == null || transition != Transition.ColorTint) return;
			image.overrideSprite = state switch {
				SelectionState.Highlighted => spriteState.highlightedSprite,
				SelectionState.Pressed => spriteState.pressedSprite,
				SelectionState.Selected => spriteState.selectedSprite,
				SelectionState.Disabled => spriteState.disabledSprite,
				_ => null
			};
		}
	}
}
