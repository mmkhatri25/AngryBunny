using UnityEngine;

namespace Lean.Touch
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class LeanSelectableSpriteRendererColor : LeanSelectableBehaviour
	{
		[Tooltip("Automatically read the DefaultColor from the SpriteRenderer?")]
		public bool AutoGetDefaultColor;

		[Tooltip("The default color given to the SpriteRenderer")]
		public Color DefaultColor = Color.white;

		[Tooltip("The color given to the SpriteRenderer when selected")]
		public Color SelectedColor = Color.green;

		protected virtual void Awake()
		{
			if (AutoGetDefaultColor)
			{
				SpriteRenderer component = GetComponent<SpriteRenderer>();
				DefaultColor = component.color;
			}
		}

		protected override void OnSelect(LeanFinger finger)
		{
			ChangeColor(SelectedColor);
		}

		protected override void OnDeselect()
		{
			ChangeColor(DefaultColor);
		}

		private void ChangeColor(Color color)
		{
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			component.color = color;
		}
	}
}
