using System;
using UnityEngine;

namespace CaramelAds
{
	public class CaramelNative
	{
		public class CaramelCallback : AndroidJavaProxy
		{
			public CaramelCallback()
				: base("com.caramelads.sdk.CaramelAdListener")
			{
			}

			public void sdkReady()
			{
				Debug.Log("CaramelUnity: try showConsentDialog");
				instance.activity.Call("runOnUiThread", new AndroidJavaRunnable(DialogOnJavaThread));
				Debug.Log("CaramelNative: Sdk ready");
				if (CaramelNative.SdkReady != null)
				{
					CaramelNative.SdkReady();
				}
				Cache();
			}

			public void sdkFailed()
			{
				Debug.Log("CaramelNative: Sdk failed: ");
				if (CaramelNative.SdkFailed != null)
				{
					CaramelNative.SdkFailed();
				}
			}

			public void adLoaded()
			{
				Debug.Log("CaramelNative: Ad loaded");
				if (CaramelNative.AdLoaded != null)
				{
					CaramelNative.AdLoaded();
				}
			}

			public void adOpened()
			{
				Debug.Log("CaramelNative: Ad opened");
				if (CaramelNative.AdOpened != null)
				{
					CaramelNative.AdOpened();
				}
			}

			public void adClicked()
			{
				Debug.Log("CaramelNative: Ad clicked");
				if (CaramelNative.AdClicked != null)
				{
					CaramelNative.AdClicked();
				}
			}

			public void adClosed()
			{
				Debug.Log("CaramelNative: Ad closed");
				if (CaramelNative.AdClosed != null)
				{
					CaramelNative.AdClosed();
				}
			}

			public void adFailed()
			{
				Debug.Log("CaramelNative: Ad failed");
				if (CaramelNative.AdFailed != null)
				{
					CaramelNative.AdFailed();
				}
			}
		}

		private AndroidJavaObject caramelAds;

		private AndroidJavaClass unityPlayer;

		private AndroidJavaObject activity;

		private AndroidJavaObject context;

		private static CaramelNative instance;

		public static event Action SdkReady;

		public static event Action AdLoaded;

		public static event Action AdFailed;

		public static event Action AdOpened;

		public static event Action AdClicked;

		public static event Action AdClosed;

		public static event Action SdkFailed;

		public static void Initialize()
		{
			if (instance == null)
			{
				instance = new CaramelNative();
				instance.caramelAds = new AndroidJavaObject("com.caramelads.sdk.CaramelAds");
				instance.unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				instance.activity = instance.unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				instance.context = instance.activity.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
				instance.caramelAds.CallStatic("initialize", instance.context);
				instance.caramelAds.CallStatic("setAdListener", new CaramelCallback());
			}
		}

		public static void Cache()
		{
			instance.caramelAds.CallStatic("cache", instance.activity);
			Debug.Log("CaramelUnity: cache");
		}

		public static bool IsLoaded()
		{
			if (instance == null)
			{
				return false;
			}
			return instance.caramelAds.CallStatic<bool>("isLoaded", new object[0]);
		}

		public static void Show()
		{
			Debug.Log("CaramelUnity: show");
			instance.activity.Call("runOnUiThread", new AndroidJavaRunnable(ShowOnJavaThread));
		}

		private static void ShowOnJavaThread()
		{
			instance.caramelAds.CallStatic("show");
		}

		private static void DialogOnJavaThread()
		{
			instance.caramelAds.CallStatic("showConsentDialog", instance.activity);
		}
	}
}
