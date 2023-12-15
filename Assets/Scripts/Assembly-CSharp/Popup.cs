using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
	public delegate void PopupAction();

	public delegate void PopupEvent(Popup popup);

	public bool disableOtherInput;

	public Text titleHolder;

	public Text messageHolder;

	public Image iconHolder;

	public PopupEvent onHidden;

	private bool m_displayed;

	private bool m_init;

	public bool displayed
	{
		get
		{
			return m_displayed;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void SetIcon(Sprite icon)
	{
		if ((bool)icon && (bool)iconHolder)
		{
			iconHolder.sprite = icon;
			ShowIcon();
		}
		else
		{
			HideIcon();
		}
	}

	private void HideIcon()
	{
		if ((bool)iconHolder)
		{
			iconHolder.gameObject.SetActive(false);
		}
	}

	private void ShowIcon()
	{
		if ((bool)iconHolder)
		{
			iconHolder.gameObject.SetActive(true);
		}
	}

	private void SetTitle(string title)
	{
		if (!string.IsNullOrEmpty(title))
		{
			if ((bool)titleHolder)
			{
				titleHolder.text = title;
			}
			ShowTitle();
		}
		else
		{
			HideTitle();
		}
	}

	private void HideTitle()
	{
		if ((bool)titleHolder)
		{
			titleHolder.gameObject.SetActive(false);
		}
	}

	private void ShowTitle()
	{
		if ((bool)titleHolder)
		{
			titleHolder.gameObject.SetActive(true);
		}
	}

	private void SetMessage(string message)
	{
		if (!string.IsNullOrEmpty(message) && messageHolder != null)
		{
			messageHolder.text = message;
			ShowMessage();
		}
		else
		{
			HideMessage();
		}
	}

	private void HideMessage()
	{
		if ((bool)messageHolder)
		{
			messageHolder.gameObject.SetActive(false);
		}
	}

	private void ShowMessage()
	{
		if ((bool)messageHolder)
		{
			messageHolder.gameObject.SetActive(true);
		}
	}

	protected virtual bool Init()
	{
		if (m_init)
		{
			return false;
		}
		m_init = true;
		base.gameObject.SetActive(false);
		return true;
	}

	protected virtual void Reset()
	{
		Init();
		HideIcon();
	}

	private void OnDisable()
	{
		onHidden = delegate
		{
		};
	}

	protected virtual void Set(string title, Sprite icon, string message, PopupEvent onHiddenCallback = null)
	{
		Reset();
		SetMessage(message);
		SetTitle(title);
		SetIcon(icon);
		onHidden = onHiddenCallback;
	}

	public virtual void Set(PopupFactory.PopupInfo info)
	{
		Set(info.title, info.icon, info.message, info.onHidden);
	}

	public virtual void Show()
	{
		StopAllCoroutines();
		base.gameObject.SetActive(true);
	}

	public virtual void Hide()
	{
		if (onHidden != null)
		{
			onHidden(this);
		}
		m_displayed = false;
		base.gameObject.SetActive(false);
	}

	public bool IsShowing()
	{
		return base.gameObject.activeSelf;
	}

	private void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Hide();
		}
	}
}
