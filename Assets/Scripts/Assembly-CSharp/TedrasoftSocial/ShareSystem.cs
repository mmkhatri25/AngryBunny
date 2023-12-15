using UnityEngine;

namespace TedrasoftSocial
{
	public class ShareSystem : MonoBehaviour
	{
		public static ShareSystem Instance;

		private void Awake()
		{
			Instance = this;
		}

		public void ShareGame()
		{
			ShareText(Application.productName, SocialSystem.Instance.GetShareLink());
		}

		public void ShareText(string subject, string body)
		{
			ShareAndroid(subject, body);
		}

		private void ShareAndroid(string subject, string body)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent");
			androidJavaObject.Call<AndroidJavaObject>("setAction", new object[1] { androidJavaClass.GetStatic<string>("ACTION_SEND") });
			androidJavaObject.Call<AndroidJavaObject>("setType", new object[1] { "text/plain" });
			androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				androidJavaClass.GetStatic<string>("EXTRA_SUBJECT"),
				subject
			});
			androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				androidJavaClass.GetStatic<string>("EXTRA_TEXT"),
				body
			});
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject2 = androidJavaClass.CallStatic<AndroidJavaObject>("createChooser", new object[2] { androidJavaObject, subject });
			@static.Call("startActivity", androidJavaObject2);
		}
	}
}
