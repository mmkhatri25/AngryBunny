using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCountUp : MonoBehaviour
{
	public float duration = 3f;

	private Text m_text;

	private void Awake()
	{
		m_text = GetComponentInChildren<Text>();
	}

	private IEnumerator CountUp(int to)
	{
		float timer = 0f;
		while (timer < duration)
		{
			timer += Time.unscaledDeltaTime;
			m_text.text = ((int)(timer / duration * (float)to)).ToString();
			yield return null;
		}
		m_text.text = to.ToString();
	}

	public void Set(int count)
	{
		StartCoroutine("CountUp", count);
	}
}
