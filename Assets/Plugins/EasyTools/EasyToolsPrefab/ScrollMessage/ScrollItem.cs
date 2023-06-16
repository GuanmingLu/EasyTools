using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools.InternalComponent {

	internal class ScrollItem : MonoBehaviour {
		private Text self;
		private void Awake() {
			self = GetComponent<Text>();
		}

		/// <summary>
		/// 显示该文字（渐变）
		/// </summary>
		/// <param name="text">文字</param>
		/// <param name="showTime">淡入时间</param>
		/// <param name="duration">显示持续时间</param>
		/// <param name="fadeOutTime">淡出时间</param>
		public void Show(string text, float showTime, float duration, float fadeOutTime) {
			self.text = text;
			self.SetA(0);
			IEnumerator C() {
				yield return EasyTween.Linear(showTime, self.SetA, self.color.a, 1);
				yield return Wait.Seconds(duration);
				yield return EasyTween.Linear(fadeOutTime, self.SetA, self.color.a, 0);
				Destroy(gameObject);
			}
			C().RunOn(this);
		}
	}
}
