using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupFactory : MonoBehaviour
{
	public delegate void PopupEvent(Popup popup);

	public enum PopupType
	{
		Message = 0,
		MessageAd = 1,
		MessageIcon = 2,
		MessageTutorial = 3
	}

	public class PopupInfo
	{
		public PopupType popupType;

		public string title;

		public string message;

		public Sprite icon;

		public Popup.PopupAction onPositiveAction;

		public string positiveButtonText;

		public Popup.PopupAction onNegativeAction;

		public string negativeButtonText;

		public int sfxClipID = -1;

		public Popup.PopupEvent onHidden;

		public PopupInfo(PopupType popupType, string title, string message, Popup.PopupEvent onHidden = null)
		{
			this.popupType = popupType;
			this.title = title;
			this.message = message;
			this.onHidden = (Popup.PopupEvent)Delegate.Combine(this.onHidden, onHidden);
		}

		public PopupInfo(PopupType popupType, string title, string message, Popup.PopupAction onPositiveAction, Popup.PopupEvent onHidden = null)
			: this(popupType, title, message, onHidden)
		{
			this.onPositiveAction = onPositiveAction;
		}

		public PopupInfo(PopupType popupType, string title, string message, Popup.PopupAction onPositiveAction, Popup.PopupAction onNegativeAction, Popup.PopupEvent onHidden = null)
			: this(popupType, title, message, onPositiveAction, onHidden)
		{
			this.onNegativeAction = onNegativeAction;
		}
	}

	public PopupEvent OnPopupCreated;

	public PopupEvent OnPopupDestroyed;

	public GameObject backgroundPanel;

	public GameObject[] popupPrefabs;

	public int queueLength = 10;

	private static PopupFactory m_instance;

	private List<Popup> m_popupsDisplayed;

	private Queue<PopupInfo> m_popupsQueue;

	private bool m_popupDisplayed;

	private int m_panelAnimatorDisplayedHash = Animator.StringToHash("Displayed");

	public static PopupFactory instance
	{
		get
		{
			if (!m_instance)
			{
				m_instance = UnityEngine.Object.FindObjectOfType<PopupFactory>();
				if (!m_instance)
				{
					Debug.LogError("Missing PopupFactory");
				}
			}
			return m_instance;
		}
		private set
		{
			m_instance = value;
		}
	}

	public Popup currentPopup
	{
		get
		{
			if (m_popupDisplayed && m_popupsDisplayed != null && m_popupsDisplayed.Count > 0 && (bool)m_popupsDisplayed[0])
			{
				return m_popupsDisplayed[0];
			}
			return null;
		}
	}

	public bool popupOrMessageActive
	{
		get
		{
			return m_popupDisplayed;
		}
	}

	public bool popupActive
	{
		get
		{
			return m_popupDisplayed;
		}
	}

	private void Awake()
	{
		m_instance = this;
		m_popupsQueue = new Queue<PopupInfo>();
		m_popupsDisplayed = new List<Popup>();
		EnableOtherInput();
	}

	private Popup DisplayPopup(PopupInfo info)
	{
		m_popupDisplayed = true;
		GameObject gameObject = UnityEngine.Object.Instantiate(popupPrefabs[(int)info.popupType], base.transform);
		gameObject.SetActive(true);
		return DisplayPopup(gameObject.GetComponentInChildren<Popup>(), info);
	}

	private Popup DisplayPopup(Popup popup, PopupInfo info)
	{
		info.onHidden = (Popup.PopupEvent)Delegate.Combine(info.onHidden, (Popup.PopupEvent)delegate(Popup pop)
		{
			HidePopup(pop);
		});
		popup.Set(info);
		DisableOtherInput(popup);
		m_popupsDisplayed.Add(popup);
		popup.Show();
		if (OnPopupCreated != null)
		{
			OnPopupCreated(popup);
		}
		return popup;
	}

	private void DisplayPopupSimple(Popup popup, PopupInfo info)
	{
		m_popupDisplayed = true;
		info.onHidden = (Popup.PopupEvent)Delegate.Combine(info.onHidden, (Popup.PopupEvent)delegate(Popup pop)
		{
			HidePopup(pop);
		});
		popup.Set(info);
		DisableOtherInput(popup);
		popup.Show();
	}

	private void HidePopup(Popup popup)
	{
		EnableOtherInput();
		DisplayNextPopup();
		m_popupsDisplayed.Remove(popup);
		if (m_popupsDisplayed.Count == 0)
		{
			m_popupDisplayed = false;
		}
		if (OnPopupDestroyed != null)
		{
			OnPopupDestroyed(popup);
		}
		UnityEngine.Object.Destroy(popup.gameObject);
	}

	private Popup DisplayNextPopup()
	{
		if (m_popupsQueue.Count > 0)
		{
			return DisplayPopup(m_popupsQueue.Dequeue());
		}
		return null;
	}

	private void DisableOtherInput(Popup popup)
	{
		if (popup.disableOtherInput)
		{
			backgroundPanel.SetActive(true);
		}
	}

	private void EnableOtherInput()
	{
		backgroundPanel.SetActive(false);
	}

	public Popup CreatePopup(PopupInfo popupInfo)
	{
		m_popupsQueue.Enqueue(popupInfo);
		if (!m_popupDisplayed)
		{
			return DisplayNextPopup();
		}
		return null;
	}

	public void CreatePopup(PopupType type)
	{
		if (m_popupDisplayed)
		{
			Popup popup = m_popupsDisplayed[m_popupsDisplayed.Count - 1];
			popup.onHidden = (Popup.PopupEvent)Delegate.Combine(popup.onHidden, (Popup.PopupEvent)delegate
			{
				CreateSimplePopup(type);
			});
		}
		else
		{
			CreateSimplePopup(type);
		}
	}

	private void CreateSimplePopup(PopupType popupType)
	{
		PopupInfo info = new PopupInfo(popupType, null, null);
		DisplayPopupSimple(InstantiatePopup(popupType), info);
	}

	public void CreatePopup(string message)
	{
		if (m_popupsQueue.Count < queueLength)
		{
			PopupInfo popupInfo = new PopupInfo(PopupType.Message, string.Empty, message);
			CreatePopup(popupInfo);
		}
	}

	public Popup GetPopup(PopupType type)
	{
		return popupPrefabs[(int)type].GetComponent<Popup>();
	}

	public Popup InstantiatePopup(PopupType type)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(popupPrefabs[(int)type], base.transform);
		return gameObject.GetComponent<Popup>();
	}

	public PopupType GetPopupType(Popup popup)
	{
		for (int i = 0; i < popupPrefabs.Length; i++)
		{
			if (popupPrefabs[i].name.Equals(popup.name))
			{
				return (PopupType)i;
			}
		}
		throw new ArgumentException("Given popup is not in the popups list.");
	}
}
