using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using D = System.Drawing;

namespace EasyTools {

	[ExecuteAlways]
	[RequireComponent(typeof(RawImage))]
	public class TextRenderer : MonoBehaviour {

		[SerializeField, TextArea(3, 10)] private string m_text;
		public string Text {
			get => m_text;
			set {
				m_text = value;
				Repaint();
			}
		}

		[SerializeField] private Color32 m_color = UnityEngine.Color.white;
		private D.Color TextColor => D.Color.FromArgb(m_color.a, m_color.r, m_color.g, m_color.b);
		[SerializeField] private float m_fontSize = 40f;
		[SerializeField] private float m_scale = 1;
		[SerializeField] private D.FontStyle m_fontStyle;
		[SerializeField] private StringAlignment m_horizontalAlignment;
		[SerializeField] private StringAlignment m_verticalAlignment;
		[SerializeField] private StringTrimming m_trimming = StringTrimming.Word;
		[SerializeField] private StringFormatFlags m_format;
		[SerializeField] private D.Text.TextRenderingHint m_renderingHint;

		private Bitmap _bitmap;
		private D.Graphics _graphics;
		private Texture2D _tex;
		private int _width, _height;

		private void OnValidate() {
			Check();
			Repaint();
		}

		private void Update() {
			if (Check()) Repaint();
		}

		private bool Check() {
			bool needRepaint = false;

			var rect = transform.ToRect().rect;
			_width = (int)(rect.width * m_scale);
			_height = (int)(rect.height * m_scale);

			_tex ??= new(_width, _height, TextureFormat.ARGB32, false);
			if (_tex.width != _width || _tex.height != _height) {
				_tex.Reinitialize(_width, _height);
				needRepaint = true;
			}

			if (_bitmap?.Width != _width || _bitmap?.Height != _height) {
				_bitmap?.Dispose();
				_bitmap = null;
				_graphics?.Dispose();
				_graphics = null;
				needRepaint = true;
			}
			_bitmap ??= new(_width, _height, PixelFormat.Format32bppArgb);
			_graphics ??= D.Graphics.FromImage(_bitmap);

			var img = GetComponent<RawImage>();
			if (_tex != img.texture) img.texture = _tex;

			return needRepaint;
		}

		private void Repaint() {
			_graphics.Clear(D.Color.Transparent);
			_graphics.TextRenderingHint = m_renderingHint;

			using (D.Font font = new(FontFamily.GenericSansSerif, m_fontSize, m_fontStyle)) {
				_graphics.DrawString(Text, font, new SolidBrush(TextColor), new RectangleF(0, 0, _width, _height), new() {
					Alignment = m_horizontalAlignment,
					LineAlignment = m_verticalAlignment,
					Trimming = m_trimming,
					FormatFlags = m_format,
				});
			}

			_graphics.Flush();

			var bitData = _bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			var length = _width * _height * 4;
			var arr = new byte[length];
			Marshal.Copy(bitData.Scan0, arr, 0, length);
			for (int i = 0; i < length; i += 4) {
				(arr[i + 3], arr[i]) = (arr[i], arr[i + 3]);
				(arr[i + 2], arr[i + 1]) = (arr[i + 1], arr[i + 2]);
			}
			_bitmap.UnlockBits(bitData);
			_tex.LoadRawTextureData(arr);
			_tex.Apply();
		}
	}
}
