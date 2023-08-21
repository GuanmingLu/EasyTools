using System.Collections;
using System.Collections.Generic;
using EasyTools.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.UI {

	public class EasySelectable : Selectable {

		[SerializeField] private bool m_changeColor, m_changeSprite, m_changeAnim;


		protected override void InstantClearState() {
			this.Reflect().TrySet("isPointerInside", false);
			this.Reflect().TrySet("isPointerDown", false);
			this.Reflect().TrySet("hasSelection", false);

			if (m_changeColor && targetGraphic != null) StartColorTween(Color.white, true);

			if (m_changeSprite) DoSpriteSwap(null);

			if (m_changeAnim) TriggerAnimation(animationTriggers.normalTrigger);
		}

		protected override void DoStateTransition(SelectionState state, bool instant) {
			if (!gameObject.activeInHierarchy)
				return;

			Color tintColor;
			Sprite transitionSprite;
			string triggerName;

			switch (state) {
				case SelectionState.Normal:
					tintColor = colors.normalColor;
					transitionSprite = null;
					triggerName = animationTriggers.normalTrigger;
					break;
				case SelectionState.Highlighted:
					tintColor = colors.highlightedColor;
					transitionSprite = spriteState.highlightedSprite;
					triggerName = animationTriggers.highlightedTrigger;
					break;
				case SelectionState.Pressed:
					tintColor = colors.pressedColor;
					transitionSprite = spriteState.pressedSprite;
					triggerName = animationTriggers.pressedTrigger;
					break;
				case SelectionState.Selected:
					tintColor = colors.selectedColor;
					transitionSprite = spriteState.selectedSprite;
					triggerName = animationTriggers.selectedTrigger;
					break;
				case SelectionState.Disabled:
					tintColor = colors.disabledColor;
					transitionSprite = spriteState.disabledSprite;
					triggerName = animationTriggers.disabledTrigger;
					break;
				default:
					tintColor = Color.black;
					transitionSprite = null;
					triggerName = string.Empty;
					break;
			}

			if (m_changeColor && targetGraphic != null) StartColorTween(tintColor * colors.colorMultiplier, instant);

			if (m_changeSprite) DoSpriteSwap(transitionSprite);

			if (m_changeAnim) TriggerAnimation(triggerName);
		}


		void StartColorTween(Color targetColor, bool instant) {
			if (targetGraphic == null) return;
			targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
		}

		void DoSpriteSwap(Sprite newSprite) {
			if (targetGraphic is Image img) img.overrideSprite = newSprite;
			else if (targetGraphic is RawImage ri) ri.texture = newSprite.texture;
		}

		void TriggerAnimation(string triggername) {
			if (animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername)) return;

			animator.ResetTrigger(animationTriggers.normalTrigger);
			animator.ResetTrigger(animationTriggers.highlightedTrigger);
			animator.ResetTrigger(animationTriggers.pressedTrigger);
			animator.ResetTrigger(animationTriggers.selectedTrigger);
			animator.ResetTrigger(animationTriggers.disabledTrigger);

			animator.SetTrigger(triggername);
		}
	}
}
