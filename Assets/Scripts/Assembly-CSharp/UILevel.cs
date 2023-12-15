using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour
{
	public delegate void SelectionEvent(int id);

	public SelectionEvent OnSelected;

	[SerializeField]
	private GameObject m_locked;

	[SerializeField]
	private GameObject m_unlocked;

	private UIStarGroup m_starGroup;

	private Text m_text;

	public int m_id;

	private void Awake()
	{
		m_starGroup = GetComponentInChildren<UIStarGroup>();
		m_text = GetComponentInChildren<Text>();
	}

	private void SetLocked(bool state)
	{
		m_locked.SetActive(state);
		m_unlocked.SetActive(!state);
	}

	public void SetLocked(int id)
	{
		m_id = id;
		SetLocked(true);
		m_starGroup.SetStatus(0);
	}

	public void SetUnlocked(int id, int stars)
	{
		m_id = id;
		m_text.text = (id + 1).ToString();
		m_starGroup.SetStatus(stars);
	}

	public void Select()
	{
		if (OnSelected != null)
		{
			OnSelected(m_id);
			GameController.SetCurrentLevelAction?.Invoke(m_id + 1);

			PlayerPrefs.SetInt("m_id", m_id+1);
        }
	}
}
