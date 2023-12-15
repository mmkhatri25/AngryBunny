using System;
using Localization;
using UnityEngine;

public class AdNetworksManager : MonoBehaviour
{
	public AdNetwork[] adNetworks;

	public string rewardedVideoSkippedMessageKey;

	public string rewardedVideoNotAvailableMessageKey;

	public string rewardedVideoFailedMessageKey;

	private static AdNetworksManager m_instance;

	private const string REWARDED_VIDEO_SAVE_KEY = "lastadnetworkREWARDED";

	private const string INTERSTITIAL_SAVE_KEY = "lastadnetworkINTERSTITIAL";

	private int m_currentAdNetworkRewarded;

	private int m_currentAdNetworkInterstitial;

	private bool m_showingRewardedVideo;

	public static AdNetworksManager instance
	{
		get
		{
			if (!m_instance)
			{
				m_instance = UnityEngine.Object.FindObjectOfType<AdNetworksManager>();
				if (!m_instance)
				{
					Debug.LogError("Missing AdNetworksManager");
				}
			}
			return m_instance;
		}
		private set
		{
			m_instance = value;
		}
	}

	public bool isShowingAd
	{
		get
		{
			return m_showingRewardedVideo;
		}
	}

	internal void ShowRewardedVideo()
	{
		throw new NotImplementedException();
	}

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		//Debug.Log("initializing AdNetworks");
		//for (int i = 0; i < adNetworks.Length; i++)
		//{
		//	adNetworks[i].Init();
		//}
	}

	private void IncreaseRewardedVideoCounter()
	{
		m_currentAdNetworkRewarded++;
		if (m_currentAdNetworkRewarded >= adNetworks.Length)
		{
			m_currentAdNetworkRewarded = 0;
		}
		SaveManager.SaveInt("lastadnetworkREWARDED", m_currentAdNetworkRewarded);
	}

	private void IncreaseInterstitialCounter()
	{
		m_currentAdNetworkInterstitial++;
		if (m_currentAdNetworkInterstitial >= adNetworks.Length)
		{
			m_currentAdNetworkInterstitial = 0;
		}
		SaveManager.SaveInt("lastadnetworkINTERSTITIAL", m_currentAdNetworkInterstitial);
	}

	public bool HasInterstitial()
	{
		for (int i = 0; i < adNetworks.Length; i++)
		{
			if (adNetworks[i].InterstitialReady())
			{
				return true;
			}
		}
		return false;
	}

	public bool ShowInterstitial(AdNetwork.AdEvent onClose)
	{
		Debug.Log("Show interstitial");
		int num = 0;
		while (num < adNetworks.Length)
		{
			num++;
			if (DisplayNextInterstitial(onClose))
			{
				return true;
			}
		}
		return false;
	}

	private bool DisplayNextInterstitial(AdNetwork.AdEvent onCloseCallback)
	{
		if (!adNetworks[m_currentAdNetworkInterstitial].InterstitialReady() || !adNetworks[m_currentAdNetworkInterstitial].isActive)
		{
			IncreaseInterstitialCounter();
			return false;
		}
		adNetworks[m_currentAdNetworkInterstitial].ShowInterstitial(onCloseCallback);
		IncreaseInterstitialCounter();
		return true;
	}

	public bool HasRewardedVideo()
	{
		for (int i = 0; i < adNetworks.Length; i++)
		{
			if (adNetworks[i].VideoReady())
			{
				return true;
			}
		}
		return false;
	}

	public bool ShowRewardedVideo(AdNetwork.AdEvent onSuccessCallback, AdNetwork.AdEvent onSkipCallback, AdNetwork.AdEvent onFailCallback)
	{
		int num = 0;
		Debug.Log("Showing rewarded!");
		while (num < adNetworks.Length)
		{
			num++;
			if (DisplayNextRewardedVideo(onSuccessCallback, onSkipCallback, onFailCallback))
			{
				return true;
			}
		}
		onFailCallback();
		VideoNotAvailablePopup();
		return false;
	}

	private bool DisplayNextRewardedVideo(AdNetwork.AdEvent onSuccessCallback, AdNetwork.AdEvent onSkipCallback, AdNetwork.AdEvent onFailCallback)
	{
		if (!adNetworks[m_currentAdNetworkRewarded].VideoReady() || !adNetworks[m_currentAdNetworkRewarded].isActive)
		{
			IncreaseRewardedVideoCounter();
			return false;
		}
		onSuccessCallback = (AdNetwork.AdEvent)Delegate.Combine(onSuccessCallback, new AdNetwork.AdEvent(EndRewardedVideo));
		onSkipCallback = (AdNetwork.AdEvent)Delegate.Combine(onSkipCallback, new AdNetwork.AdEvent(EndRewardedVideo));
		onFailCallback = (AdNetwork.AdEvent)Delegate.Combine(onFailCallback, new AdNetwork.AdEvent(EndRewardedVideo));
		onSkipCallback = (AdNetwork.AdEvent)Delegate.Combine(onSkipCallback, new AdNetwork.AdEvent(VideoSkippedPopup));
		onFailCallback = (AdNetwork.AdEvent)Delegate.Combine(onFailCallback, new AdNetwork.AdEvent(VideoFailedPopup));
		m_showingRewardedVideo = true;
		Debug.Log("Playing video from: " + adNetworks[m_currentAdNetworkRewarded]);
		adNetworks[m_currentAdNetworkRewarded].ShowVideo(onSuccessCallback, onSkipCallback, onFailCallback);
		IncreaseRewardedVideoCounter();
		return true;
	}

	private void VideoNotAvailablePopup()
	{
		PopupFactory.instance.CreatePopup(LanguageManager.Get(rewardedVideoNotAvailableMessageKey));
	}

	private void VideoSkippedPopup()
	{
		PopupFactory.instance.CreatePopup(LanguageManager.Get(rewardedVideoSkippedMessageKey));
	}

	private void VideoFailedPopup()
	{
	}

	private void EndRewardedVideo()
	{
		m_showingRewardedVideo = false;
	}
}
