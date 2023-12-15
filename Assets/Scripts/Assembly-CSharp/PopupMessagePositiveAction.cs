using UnityEngine.UI;

public class PopupMessagePositiveAction : Popup
{
	public Button positiveActionButton;

	private string m_initPositiveActionbuttonText;

	private Text m_positiveActionbuttonText;

	protected override bool Init()
	{
		if (!base.Init())
		{
			return false;
		}
		m_positiveActionbuttonText = positiveActionButton.GetComponentInChildren<Text>();
		if ((bool)m_positiveActionbuttonText)
		{
			m_initPositiveActionbuttonText = m_positiveActionbuttonText.text;
		}
		return true;
	}

	protected override void Reset()
	{
		base.Reset();
		if ((bool)m_positiveActionbuttonText)
		{
			m_positiveActionbuttonText.text = m_initPositiveActionbuttonText;
		}
	}

	public override void Set(PopupFactory.PopupInfo info)
	{
		base.Set(info);
		positiveActionButton.onClick.RemoveAllListeners();
		positiveActionButton.onClick.AddListener(delegate
		{
			if (info.onPositiveAction != null)
			{
				info.onPositiveAction();
			}
			AudioController.PlaySFX(0);
			Hide();
		});
		SetPositiveButtonText(info.positiveButtonText);
	}

	public void SetPositiveButtonText(string text)
	{
		if (text != null && (bool)m_positiveActionbuttonText)
		{
			m_positiveActionbuttonText.text = text;
		}
	}
}
