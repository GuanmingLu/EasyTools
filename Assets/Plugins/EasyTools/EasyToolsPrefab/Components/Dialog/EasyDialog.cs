using UnityEngine;
using EasyTools.InternalComponent;
using System.Collections.Generic;

namespace EasyTools {

	public static class EasyDialog {
		private static EasyDialogComponent _instance;
		private static EasyDialogComponent Instance => _instance ??= EasyToolsGameObject.Instance.GetComponentInChildren<EasyDialogComponent>(true);


		private static Coroutine _dialogCoroutine;
		public static Coroutine ShowDialogues(string fileName) {
			Instance.ShowDialogues(fileName).Run(ref _dialogCoroutine);
			return _dialogCoroutine;
		}
		public static Coroutine ShowDialogues(IEnumerable<EasyDialogContent> dialogContents) {
			Instance.ShowDialogues(dialogContents).Run(ref _dialogCoroutine);
			return _dialogCoroutine;
		}

	}


	public struct EasyDialogContent {
		public string name;
		public string content;
	}
}
