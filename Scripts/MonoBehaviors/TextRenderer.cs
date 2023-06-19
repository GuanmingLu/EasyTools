using D = System.Drawing;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using EasyTools.Inspector;
using System.IO;
using System.Diagnostics;

namespace EasyTools {

	[ExecuteAlways]
	[RequireComponent(typeof(CanvasRenderer))]
	public class TextRenderer : MaskableGraphic {
		[SerializeField, TextArea(3, 10)] private string m_text;
		public string Text {
			get => m_text;
			set {
				m_text = value;
				Repaint();
			}
		}

		[SerializeField] private StreamingAssetFile m_fontFile = new();
		// [SerializeField] private string m_fontName = "微软雅黑";
		[SerializeField, Min(0.001f)] private float m_fontSize = 40f;
		[SerializeField, Min(0.001f)] private float m_superSample = 1;
		private float RealFontSize => m_fontSize * m_superSample;
		[SerializeField] private D.FontStyle m_fontStyle;
		[SerializeField] private StringAlignment m_horizontalAlignment;
		[SerializeField] private StringAlignment m_verticalAlignment;
		[SerializeField] private StringTrimming m_trimming;
		[SerializeField] private StringFormatFlags m_format;
		[SerializeField] private TextRenderingHint m_renderingHint;

		private Bitmap _bitmap;
		private D.Graphics _graphics;
		private Texture2D _tex;
		public override Texture mainTexture => _tex;
		private int _width, _height;

		protected override void OnPopulateMesh(VertexHelper vh) {
			vh.Clear();
			if (_tex != null) {
				var r = GetPixelAdjustedRect();
				var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
				var scaleX = _tex.width * _tex.texelSize.x;
				var scaleY = _tex.height * _tex.texelSize.y;
				{
					var color32 = color;
					vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(0, 0));
					vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(0, -scaleY));
					vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(scaleX, -scaleY));
					vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(scaleX, 0));

					vh.AddTriangle(0, 1, 2);
					vh.AddTriangle(2, 3, 0);
				}
			}
		}

		protected override void OnValidate() {
			base.OnValidate();
			Check();
			Repaint();
		}

		private void Update() {
			if (Check()) Repaint();
		}

		private bool Check() {
			bool needRepaint = false;

			var rect = rectTransform.rect;

			m_superSample = Mathf.Clamp(m_superSample, 8 / rect.width, 8192 / rect.width);
			m_superSample = Mathf.Clamp(m_superSample, 8 / rect.height, 8192 / rect.height);

			_width = (int)Mathf.Max(1, rect.width * m_superSample);
			_height = (int)Mathf.Max(1, rect.height * m_superSample);

			_tex ??= new(_width, _height, TextureFormat.BGRA32, false);
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

			return needRepaint;
		}

		private void Repaint() {
			_graphics.Clear(D.Color.Transparent);
			_graphics.TextRenderingHint = m_renderingHint;

			D.Font font;
			if (File.Exists(m_fontFile)) {
				using PrivateFontCollection pfc = new();
				pfc.AddFontFile(m_fontFile);
				font = new(pfc.Families[0], RealFontSize, m_fontStyle);
			}
			else {
				font = new(FontFamily.GenericSansSerif, RealFontSize, m_fontStyle);
			}

			_graphics.DrawString(Text, font, new SolidBrush(D.Color.White), new RectangleF(0, 0, _width, _height), new() {
				Alignment = m_horizontalAlignment,
				LineAlignment = m_verticalAlignment,
				Trimming = m_trimming,
				FormatFlags = m_format,
			});
			font.Dispose();

			_graphics.Flush();

			var bitData = _bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			_tex.LoadRawTextureData(bitData.Scan0, _width * _height * 4);
			_bitmap.UnlockBits(bitData);
			_tex.Apply();
		}
	}
}
