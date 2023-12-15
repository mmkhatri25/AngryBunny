using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	public float force;

	public float radius;

	public LayerMask collisionMask;

	public float scaleMultiplier = 1f;

	public float lifeTime = 2f;

	public IAudioEvent audioEvent;

	private bool m_exploded;

	public void Explode()
	{
		if (!m_exploded)
		{
			m_exploded = true;
			base.transform.localScale *= radius * scaleMultiplier;
			ApplyForce();
			audioEvent.Play(GetComponent<AudioSource>(), AudioController.Volume);
			StartCoroutine("Destroy", lifeTime);
		}
	}

	private void ApplyForce()
	{
		RaycastHit2D[] array = Physics2D.CircleCastAll(base.transform.position, radius, Vector2.zero, radius, collisionMask);
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			Breakable breakable = array[i].collider.GetComponentInChildren<Breakable>();
			if (!breakable)
			{
				breakable = array[i].collider.GetComponentInParent<Breakable>();
			}
			if ((bool)breakable)
			{
				Vector2 vector = array[i].transform.position - base.transform.position;
				breakable.ApplyForce(force * 1f);
				Rigidbody2D componentInChildren = array[i].collider.gameObject.GetComponentInChildren<Rigidbody2D>();
				if (!componentInChildren)
				{
					array[i].collider.gameObject.GetComponentInParent<Rigidbody2D>();
				}
				if ((bool)componentInChildren)
				{
					componentInChildren.AddForce(vector * force, ForceMode2D.Impulse);
				}
			}
		}
		GameController.Vibrate();
	}

	private void Update()
	{
		base.transform.localScale = Vector3.one * (radius * scaleMultiplier);
	}

	private IEnumerator Destroy(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Object.Destroy(base.gameObject);
	}
}
