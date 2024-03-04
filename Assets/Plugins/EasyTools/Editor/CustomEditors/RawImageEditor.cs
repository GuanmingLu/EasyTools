using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

namespace EasyTools.Editor {
	[CustomEditor(typeof(RawImage), true)]
	[CanEditMultipleObjects]
	public class RawImageEditor : UnityEditor.UI.RawImageEditor {
		private RectTransform _rectTransform;
		private GUIContent _buttonContent = EditorGUIUtility.TrTextContent("Set Real Size", "Sets the size to match the image.");

		protected override void OnEnable() {
			base.OnEnable();
			_rectTransform = ((RawImage)target).rectTransform;
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			RealSizeGUI();
		}

		private void RealSizeGUI() {
			var tex = ((RawImage)target).mainTexture;
			if (tex == null) return;

			var path = AssetDatabase.GetAssetPath(tex);
			if (string.IsNullOrEmpty(path)) return;

			var absPath = Path.Combine(Directory.GetCurrentDirectory(), path);
			if (!File.Exists(absPath)) return;

			int w = 0, h = 0;
			try {
				using var stream = File.OpenRead(absPath);
				using var sourceImage = System.Drawing.Image.FromStream(stream, false, false);
				w = sourceImage.Width;
				h = sourceImage.Height;
			}
			catch {
				return;
			}

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(EditorGUIUtility.labelWidth);
				if (GUILayout.Button(_buttonContent, EditorStyles.miniButton)) {
					foreach (Graphic graphic in targets.Select(obj => obj as Graphic)) {
						Undo.RecordObject(graphic.rectTransform, "Set Real Size");
						_rectTransform.anchorMax = _rectTransform.anchorMin;
						_rectTransform.sizeDelta = new Vector2(w, h);
						EditorUtility.SetDirty(graphic);
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
