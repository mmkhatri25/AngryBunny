using System.Collections;
using UnityEngine;

public class AnimatorStartDelay : MonoBehaviour
{
	public Vector2 limits;

	private Animator m_animator;

	private void Awake()
	{
		m_animator = GetComponentInChildren<Animator>();
		m_animator.enabled = false;
		StartCoroutine("EnableAnimator", Random.Range(limits.x, limits.y));
	}

	private IEnumerator EnableAnimator(float delay)
	{
		float timer = 0f;
		while (timer < delay)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		m_animator.enabled = true;
	}
}
