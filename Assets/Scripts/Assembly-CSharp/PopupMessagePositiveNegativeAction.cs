using UnityEngine.UI;

public class PopupMessagePositiveNegativeAction : PopupMessagePositiveAction
{
	public Button negativeActionButton;

	private string m_initNegativeActionButtonText;

	private Text m_negativeActionbuttonText;

	protected override bool Init()
	{
		if (!base.Init())
		{
			return false;
		}
		if (!m_negativeActionbuttonText)
		{
			return true;
		}
		m_negativeActionbuttonText = negativeActionButton.GetComponentInChildren<Text>();
		m_initNegativeActionButtonText = m_negativeActionbuttonText.text;
		return true;
	}

	protected override void Reset()
	{
		base.Reset();
		if ((bool)m_negativeActionbuttonText)
		{
			m_negativeActionbuttonText.text = m_initNegativeActionButtonText;
		}
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void Set(PopupFactory.PopupInfo info)
	{
		base.Set(info);
		negativeActionButton.onClick.RemoveAllListeners();
		negativeActionButton.onClick.AddListener(delegate
		{
			if (info.onNegativeAction != null)
			{
				info.onNegativeAction();
			}
			//AdNetworkCaramel.bunnies = false;
			//AdNetworkCaramel.quakes = false;
			AudioController.PlaySFX(0);
			Hide();
		});
		SetNegativeButtonText(info.negativeButtonText);
	}

	public void SetNegativeButtonText(string text)
	{
		if (text != null)
		{
			m_negativeActionbuttonText.text = text;
		}
	}
}
