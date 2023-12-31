using System.Linq;
using UnityEngine;
using EasyTools.Reflection;

namespace EasyTools.InternalComponent {

	[ExecuteAlways]
	public class EasyLocalizationText : MonoBehaviour {
		public UnityEngine.Object target;
		public string propertyName = "text", fileName = "", key = "";

		private void Reset() {
			if (TryGetComponent<UnityEngine.UI.Text>(out var t)) target = t;
			else if (TryGetComponent<TextMesh>(out var tm)) target = tm;
			else if (TryGetComponent<TMPro.TMP_Text>(out var tmp)) target = tmp;

			Refresh();
			EasyLocalization.OnLangSwitched -= Refresh;
			EasyLocalization.OnLangSwitched += Refresh;
		}

		private void Start() {
			if (Application.IsPlaying(gameObject)) {
				Refresh();
				EasyLocalization.OnLangSwitched -= Refresh;
				EasyLocalization.OnLangSwitched += Refresh;
			}
		}

		private void OnDestroy() {
			EasyLocalization.OnLangSwitched -= Refresh;
		}

		public void Refresh() {
			if (target != null && !string.IsNullOrWhiteSpace(propertyName) && EasyLocalization.TryGet<string>(fileName, key, out var s)) {
				target.Reflect().TrySet(propertyName, s);
			}
		}
	}
}
