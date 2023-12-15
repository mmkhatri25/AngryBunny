using UnityEngine;

public class AdNetwork : MonoBehaviour
{
	public delegate void AdEvent();

	public AdEvent onSuccess;

	public AdEvent onSkip;

	public AdEvent onFail;

	public AdEvent onClose;

	public bool isActive = true;

	public void SetActive(bool state)
	{
		isActive = state;
	}

	public virtual void Init()
	{
	}

	public virtual bool InterstitialReady()
	{
		return false;
	}

	public virtual void ShowInterstitial(AdEvent onCloseCallback)
	{
		onClose = onCloseCallback;
	}

	public virtual bool InterstitialShowing()
	{
		return false;
	}

	public virtual bool VideoReady()
	{
		return false;
	}

	public virtual void ShowVideo(AdEvent onSuccessCallback, AdEvent onCancelCallback, AdEvent onFailCallback)
	{
		onSuccess = onSuccessCallback;
		onSkip = onCancelCallback;
		onFail = onFailCallback;
	}

	public virtual bool VideoShowing()
	{
		return false;
	}
}
