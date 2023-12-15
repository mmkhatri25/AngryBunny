using System.Collections;
using UnityEngine;

namespace TedrasoftSocial
{
	public class SocialSystem : MonoBehaviour
	{
		public string AndroidPackageName = "com.company.game";

		public string IOSAppId = "Get it from itunes connect";

		private string shareLinkAndroid = "https://play.google.com/store/apps/details?id=";

		private string shareLinkiOS = "https://itunes.apple.com/app/id";

		public string ShareLinkOtherPlatform = string.Empty;

		public string MailSubject = "Game Title";

		public string Email = "office@tedrasoft.com";

		public string SubjectNormalRate = "Normal rate";

		public string SubjectNegativeRate = "Negative rate";

		[SerializeField]
		private Transform rateCanvasAndroid;

		[SerializeField]
		private Transform rateCanvasIOS;

		public static SocialSystem Instance { get; private set; }

		public bool RateInProgress { get; private set; }

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else if (Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void Start()
		{
			shareLinkAndroid += AndroidPackageName;
			shareLinkiOS += IOSAppId;
		}

		public string GetShareLink()
		{
			return shareLinkAndroid;
		}

		public void RatePopUp()
		{
			if (PlayerPrefs.GetInt("Rated") != 1)
			{
				OpenRatePanel();
			}
		}

		public void OpenRatePanel()
		{
			RateInProgress = true;
			rateCanvasAndroid.gameObject.SetActive(true);
		}

		public void CloseRatePanel()
		{
			rateCanvasAndroid.gameObject.SetActive(false);
			StartCoroutine(DelayedState(1f));
		}

		public void PositiveFeedback()
		{
			PlayerPrefs.SetInt("Rated", 1);
			Application.OpenURL(shareLinkAndroid);
			CloseRatePanel();
		}

		private IEnumerator DelayedState(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			RateInProgress = false;
		}

		public void NeutralFeedback()
		{
			PlayerPrefs.SetInt("Rated", 1);
			SendEmail(Email, SubjectNormalRate);
			CloseRatePanel();
		}

		public void NegativeFeedback()
		{
			PlayerPrefs.SetInt("Rated", 1);
			SendEmail(Email, SubjectNegativeRate);
			CloseRatePanel();
		}

		private void SendEmail(string email, string subject)
		{
			subject = string.Format("{0} ({1}) - {2}", MailSubject, Application.platform.ToString(), subject);
			subject = MyEscapeURL(subject);
			Application.OpenURL("mailto:" + email + "?subject=" + subject);
		}

		private string MyEscapeURL(string url)
		{
			return WWW.EscapeURL(url).Replace("+", "%20");
		}
	}
}
