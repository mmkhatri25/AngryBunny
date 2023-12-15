using UnityEngine;

public class Score : MonoBehaviour
{
	[SerializeField]
	private int m_score;

	[SerializeField]
	private Color m_color;

	[SerializeField]
	private int m_size;

	public int ScoreAmount
	{
		get
		{
			return m_score;
		}
	}

	public void Show(int amount)
	{
		ScoreTextController.SpawnText(base.transform, amount.ToString(), m_color, m_size);
	}
}
