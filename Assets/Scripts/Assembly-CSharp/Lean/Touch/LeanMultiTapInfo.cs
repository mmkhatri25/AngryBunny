using UnityEngine;
using UnityEngine.UI;

namespace Lean.Touch
{
	public class LeanMultiTapInfo : MonoBehaviour
	{
		[Tooltip("The Text element you want to display the info in")]
		public Text Text;

		public void OnMultiTap(int count, int highest)
		{
			if (Text != null)
			{
				Text.text = "You just multi-tapped " + count + " time(s) with " + highest + " finger(s)";
			}
		}
	}
}
