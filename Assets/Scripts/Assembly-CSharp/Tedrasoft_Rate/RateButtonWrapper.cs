using TedrasoftSocial;
using UnityEngine;
using UnityEngine.UI;

namespace Tedrasoft_Rate
{
	public class RateButtonWrapper : MonoBehaviour
	{
		private Button btn;

		public void Start()
		{
			btn = GetComponent<Button>();
			btn.onClick.AddListener(delegate
			{
				if (SocialSystem.Instance != null)
				{
					SocialSystem.Instance.PositiveFeedback();
				}
			});
		}
	}
}
