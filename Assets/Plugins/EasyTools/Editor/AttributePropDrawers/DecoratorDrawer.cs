using EasyTools.Inspector;
using UnityEditor;
using UnityEngine;

namespace EasyTools.Editor {
	public abstract class DecoratorDrawerBase<T> : DecoratorDrawer where T : PropertyAttribute {
		private T _self;
		protected T Self {
			get {
				if (_self == null) _self = (T)attribute;
				return _self;
			}
		}
	}

	[CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
	public class HorizontalLineAttributeDrawer : DecoratorDrawerBase<HorizontalLineAttribute> {
		public override float GetHeight() => Utils.SingleLinePropHeight;
		public override void OnGUI(Rect position) => Utils.HorizontalLine(position, Self.height, Self.color);
	}
}
