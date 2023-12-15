using UnityEngine;
using UnityEngine.UI;

namespace Tedrasoft_Rate
{
	public class StarScript : MonoBehaviour
	{
		[SerializeField]
		private Image[] stars;

		[SerializeField]
		private Color pressedColor;

		[SerializeField]
		private Color normalColor;

		public void OnMouseOverStart()
		{
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i].color = pressedColor;
			}
		}

		public void OnMouseOverEnd()
		{
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i].color = normalColor;
			}
		}
	}
}
