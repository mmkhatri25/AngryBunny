using System;
using System.Collections.Generic;
using MiniJSONV;
using UnityEngine;

public class VungleManager : MonoBehaviour
{
	private static AdFinishedEventArgs adWinFinishedEventArgs;

	public static event Action<string> OnAdStartEvent;

	public static event Action<string, bool> OnAdPlayableEvent;

	public static event Action<string> OnSDKLogEvent;

	public static event Action<string, string> OnPlacementPreparedEvent;

	public static event Action<string, string> OnVungleCreativeEvent;

	public static event Action<string, AdFinishedEventArgs> OnAdFinishedEvent;

	public static event Action OnSDKInitializeEvent;

	static VungleManager()
	{
		try
		{
			GameObject gameObject = new GameObject("VungleManager");
			gameObject.AddComponent<VungleManager>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		catch (UnityException)
		{
			Debug.LogWarning("It looks like you have the VungleManager on a GameObject in your scene. Please remove the script from your scene.");
		}
	}

	public static void noop()
	{
	}

	public static void onEvent(string e, string arg)
	{
		if (e == "OnAdStart")
		{
			VungleManager.OnAdStartEvent(arg);
		}
		if (e == "OnAdEnd")
		{
			adWinFinishedEventArgs = new AdFinishedEventArgs();
			string[] array = arg.Split(':');
			adWinFinishedEventArgs.WasCallToActionClicked = "1".Equals(array[0]);
			adWinFinishedEventArgs.IsCompletedView = bool.Parse(array[2]);
			adWinFinishedEventArgs.TimeWatched = double.Parse(array[3]) / 1000.0;
			VungleManager.OnAdFinishedEvent(array[1], adWinFinishedEventArgs);
		}
		if (e == "OnAdPlayableChanged")
		{
			string[] array2 = arg.Split(':');
			VungleManager.OnAdPlayableEvent(array2[1], "1".Equals(array2[0]));
		}
		if (e == "Diagnostic")
		{
			VungleManager.OnSDKLogEvent(arg);
		}
		if (e == "OnInitCompleted" && "1".Equals(arg))
		{
			VungleManager.OnSDKInitializeEvent();
		}
	}

	private void OnAdStart(string placementID)
	{
		VungleManager.OnAdStartEvent(placementID);
	}

	private void OnAdPlayable(string param)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(param);
		bool arg = extractBoolValue(dictionary, "isAdAvailable");
		string arg2 = dictionary["placementID"].ToString();
		VungleManager.OnAdPlayableEvent(arg2, arg);
	}

	private void OnVideoView(string param)
	{
	}

	private void OnSDKLog(string log)
	{
		VungleManager.OnSDKLogEvent(log);
	}

	private void OnPlacementPrepared(string param)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(param);
		string arg = dictionary["placementID"].ToString();
		string arg2 = dictionary["bidToken"].ToString();
		VungleManager.OnPlacementPreparedEvent(arg, arg2);
	}

	private void OnVungleCreative(string param)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(param);
		string arg = dictionary["placementID"].ToString();
		string arg2 = dictionary["creativeID"].ToString();
		VungleManager.OnVungleCreativeEvent(arg, arg2);
	}

	private void OnInitialize(string empty)
	{
		VungleManager.OnSDKInitializeEvent();
	}

	private void OnAdEnd(string param)
	{
		AdFinishedEventArgs adFinishedEventArgs = new AdFinishedEventArgs();
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(param);
		adFinishedEventArgs.WasCallToActionClicked = extractBoolValue(dictionary, "wasCallToActionClicked");
		adFinishedEventArgs.IsCompletedView = extractBoolValue(dictionary, "wasSuccessFulView");
		adFinishedEventArgs.TimeWatched = 0.0;
		VungleManager.OnAdFinishedEvent(dictionary["placementID"].ToString(), adFinishedEventArgs);
	}

	private bool extractBoolValue(string json, string key)
	{
		Dictionary<string, object> attrs = (Dictionary<string, object>)Json.Deserialize(json);
		return extractBoolValue(attrs, key);
	}

	private bool extractBoolValue(Dictionary<string, object> attrs, string key)
	{
		return bool.Parse(attrs[key].ToString());
	}
}
