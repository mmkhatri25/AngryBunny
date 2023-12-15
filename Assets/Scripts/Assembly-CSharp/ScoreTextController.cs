using System.Collections.Generic;
using UnityEngine;

public class ScoreTextController : MonoBehaviour
{
	[SerializeField]
	private GameObject m_textPrefab;

	[SerializeField]
	private Transform m_parent;

	private List<ScoreText> m_list;

	private const int m_size = 30;

	private int m_index;

	private static ScoreTextController m_instance;

	private static ScoreTextController Instance
	{
		get
		{
			if (!m_instance)
			{
				m_instance = Object.FindObjectOfType<ScoreTextController>();
			}
			return m_instance;
		}
	}

	private void Awake()
	{
		m_list = new List<ScoreText>();
		for (int i = 0; i < 30; i++)
		{
			GameObject gameObject = Object.Instantiate(m_textPrefab, m_parent);
			m_list.Add(gameObject.GetComponentInChildren<ScoreText>());
			gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < m_list.Count; i++)
		{
			m_list[i].gameObject.SetActive(false);
		}
	}

	private void InternalSpawnText(Transform target, string text, Color color, int size)
	{
		m_list[Instance.m_index].gameObject.SetActive(true);
		m_list[Instance.m_index].Set(target, text, color, size);
		m_index++;
		if (m_index >= 30)
		{
			m_index = 0;
		}
	}

	public static void SpawnText(Transform target, string text, Color color, int size)
	{
		Instance.InternalSpawnText(target, text, color, size);
	}
}
