using System;
using System.Collections.Generic;
using CaramelAds;
using UnityEngine;
using UnityEngine.UI;

public class CaramelDemo : MonoBehaviour
{
	[SerializeField]
	private Text adsStatus;

	[SerializeField]
	private Text eventFeed;

	private Queue<Action> logQueue = new Queue<Action>();

	private void Awake()
	{
		eventFeed.text = string.Empty;
	}

	private void Start()
	{
		CaramelNative.SdkReady += delegate
		{
			AddText("Caramel: Sdk Ready");
		};
		CaramelNative.SdkFailed += delegate
		{
			AddText("Caramel: Sdk Failed: ");
		};
		CaramelNative.AdLoaded += delegate
		{
			AddText("Caramel: Ad loaded");
		};
		CaramelNative.AdClicked += delegate
		{
			AddText("Caramel: Ad clicked");
		};
		CaramelNative.AdOpened += delegate
		{
			AddText("Caramel: Ad opened");
		};
		CaramelNative.AdFailed += delegate
		{
			AddText("Caramel: Ad failed");
		};
		CaramelNative.AdClosed += delegate
		{
			AddText("Caramel: Ad closed");
		};
		CaramelNative.Initialize();
	}

	public void ShowAds()
	{
		AddText("Trying to display ad...");
		if (CaramelNative.IsLoaded())
		{
			CaramelNative.Show();
		}
		else
		{
			AddText("Add not loaded. Wait for Status: Ad ready");
		}
	}

	public void Cache()
	{
		if (!CaramelNative.IsLoaded())
		{
			CaramelNative.Cache();
			AddText("Caching...");
		}
	}

	private void Update()
	{
		while (logQueue.Count > 0)
		{
			Action action = logQueue.Dequeue();
			if (action != null)
			{
				action();
			}
		}
		if (CaramelNative.IsLoaded())
		{
			adsStatus.text = "Status: Ad ready";
		}
		else
		{
			adsStatus.text = "Status: Ad not ready";
		}
	}

	private void AddText(string text)
	{
		lock (logQueue)
		{
			logQueue.Enqueue(delegate
			{
				Text text2 = eventFeed;
				text2.text = text2.text + text + "\n";
				Debug.Log("Logging: " + text + "\n");
			});
		}
	}
}
