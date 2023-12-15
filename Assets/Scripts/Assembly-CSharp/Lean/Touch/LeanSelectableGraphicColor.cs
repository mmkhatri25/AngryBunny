using UnityEngine;
using UnityEngine.UI;

namespace Lean.Touch
{
	[RequireComponent(typeof(Graphic))]
	public class LeanSelectableGraphicColor : LeanSelectableBehaviour
	{
		[Tooltip("Automatically read the DefaultColor from the Renderer.material?")]
		public bool AutoGetDefaultColor;

		[Tooltip("The default color given to the Renderer.material")]
		public Color DefaultColor = Color.white;

		[Tooltip("The color given to the Renderer.material when selected")]
		public Color SelectedColor = Color.green;

		protected virtual void Start()
		{
			if (AutoGetDefaultColor)
			{
				Graphic component = GetComponent<Graphic>();
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
			Graphic component = GetComponent<Graphic>();
			component.color = color;
		}
	}
}
