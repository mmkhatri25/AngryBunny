using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStarGroup : MonoBehaviour
{
	[SerializeField]
	protected List<Image> m_stars;

	[SerializeField]
	protected Sprite m_unlocked;

	[SerializeField]
	protected Sprite m_locked;

	public virtual void SetStatus(int count)
	{
		if (count > m_stars.Count)
		{
			count = m_stars.Count;
		}
		for (int i = 0; i < m_stars.Count; i++)
		{
			m_stars[i].sprite = ((i >= count) ? m_locked : m_unlocked);
		}
	}
}
