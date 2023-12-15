using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
	private Text m_text;

	private Vector3 m_target;

	private Camera m_camera;

	private Camera Camera
	{
		get
		{
			if (!m_camera)
			{
				m_camera = Object.FindObjectOfType<Camera>();
			}
			return m_camera;
		}
	}

	private Text Text
	{
		get
		{
			if (!m_text)
			{
				m_text = GetComponentInChildren<Text>();
			}
			return m_text;
		}
	}

	public void Set(Transform target, string text, Color color, int size)
	{
		m_target = target.position;
		base.transform.position = Camera.WorldToScreenPoint(m_target);
		Text.text = text;
		Text.color = color;
		Text.fontSize = size;
	}

	private void Update()
	{
		base.transform.position = Camera.WorldToScreenPoint(m_target);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}
}
