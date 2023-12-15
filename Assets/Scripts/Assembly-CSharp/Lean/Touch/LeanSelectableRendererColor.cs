using System;
using UnityEngine;

namespace Lean.Touch
{
	[RequireComponent(typeof(Renderer))]
	public class LeanSelectableRendererColor : LeanSelectableBehaviour
	{
		[Tooltip("Automatically read the DefaultColor from the material?")]
		public bool AutoGetDefaultColor;

		[Tooltip("The default color given to the materials")]
		public Color DefaultColor = Color.white;

		[Tooltip("The color given to the materials when selected")]
		public Color SelectedColor = Color.green;

		[Tooltip("Should the materials get cloned at the start?")]
		public bool CloneMaterials = true;

		[NonSerialized]
		private Renderer cachedRenderer;

		protected virtual void Start()
		{
			if (cachedRenderer == null)
			{
				cachedRenderer = GetComponent<Renderer>();
			}
			if (AutoGetDefaultColor)
			{
				Material sharedMaterial = cachedRenderer.sharedMaterial;
				if (sharedMaterial != null)
				{
					DefaultColor = sharedMaterial.color;
				}
			}
			if (CloneMaterials)
			{
				cachedRenderer.sharedMaterials = cachedRenderer.materials;
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
			if (cachedRenderer == null)
			{
				cachedRenderer = GetComponent<Renderer>();
			}
			Material[] sharedMaterials = cachedRenderer.sharedMaterials;
			for (int num = sharedMaterials.Length - 1; num >= 0; num--)
			{
				sharedMaterials[num].color = color;
			}
		}
	}
}
