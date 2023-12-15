using UnityEngine.UI;

public class ExtraProjectilesButton : GameplayButton
{
	private Text m_Text;

	private Button m_Button;

	private void OnEnable()
	{
		Refresh();
	}

	public override void Refresh()
	{
		if (!m_Text)
		{
			m_Text = GetComponentInChildren<Text>();
		}
		m_Text.text = GameStats.ExtraProjectilesCount.ToString();
		if (!m_Button)
		{
			m_Button = GetComponentInChildren<Button>();
		}
		m_Button.interactable = GameStats.ExtraProjectilesCount > 0;
	}
}
