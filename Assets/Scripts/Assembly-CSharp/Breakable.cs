using System.Collections;
using UnityEngine;

public class Breakable : MonoBehaviour
{
	public enum Type
	{
		Glass = 0,
		Wood = 1,
		Rock = 2
	}

	public delegate void BreakableEvent();

	public delegate void BreakableObjectEvent(GameObject breakable);

	private struct InternanalRay
	{
		public Vector3 origin;

		public Vector3 direction;

		public float distance;
	}

	public BreakableEvent OnCrash;

	public BreakableObjectEvent OnDestroyed;

	[SerializeField]
	private Sprite[] m_damagedSprites;

	[SerializeField]
	private float m_endurance;

	[SerializeField]
	private ParticleSystem m_crashPS;

	[SerializeField]
	private bool m_objective;

	[SerializeField]
	private IAudioEvent m_hitAudioEvent;

	[SerializeField]
	private IAudioEvent m_idleAudioEvent;

	[SerializeField]
	private IAudioEvent m_destroyedAudioEvent;

	[SerializeField]
	private Vector2 m_idleAnimationsCooldown;

	[SerializeField]
	private LayerMask m_kinematicMask;

	[SerializeField]
	private Type m_type;

	private bool m_active = true;

	private float m_idleCounter;

	private float m_currentIdleTimer = 4f;

	private int m_currentSpriteIndex = -1;

	private SpriteRenderer m_spriteRenderer;

	private AudioSource m_audioSource;

	private const float m_neightborDistance = 0.25f;

	public bool Objective
	{
		get
		{
			return m_objective;
		}
	}

	public bool Active
	{
		get
		{
			return m_active;
		}
	}

	private void Awake()
	{
		m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		m_audioSource = GetComponentInChildren<AudioSource>();
		m_currentIdleTimer = Random.Range(m_idleAnimationsCooldown.x, m_idleAnimationsCooldown.y);
	}

	public void CancelKinematic()
	{
		Rigidbody2D componentInChildren = GetComponentInChildren<Rigidbody2D>();
		if (!componentInChildren.isKinematic)
		{
			return;
		}
		componentInChildren.isKinematic = false;
		Vector3 vector = GetComponentInChildren<Collider2D>().bounds.size + Vector3.one * 0.25f;
		RaycastHit2D[] array = Physics2D.BoxCastAll(base.transform.position, vector, 0f, Vector3.zero);
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			Breakable componentInParent = array[i].collider.GetComponentInParent<Breakable>();
			if (!(componentInParent == null) && !(componentInParent == this))
			{
				componentInParent.CancelKinematic();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		ProjectileController componentInChildren = collision.collider.GetComponentInChildren<ProjectileController>();
		float num = ((!componentInChildren) ? 1f : componentInChildren.GetDamageMultiplier(m_type));
		float force = collision.relativeVelocity.magnitude * num;
		ApplyForce(force, componentInChildren);
	}

	private void Update()
	{
		if ((bool)m_idleAudioEvent)
		{
			m_idleCounter += Time.deltaTime;
			if (m_idleCounter >= m_currentIdleTimer)
			{
				m_idleCounter = 0f;
				m_currentIdleTimer = Random.Range(m_idleAnimationsCooldown.x, m_idleAnimationsCooldown.y);
				m_idleAudioEvent.Play(m_audioSource, AudioController.Volume);
			}
		}
	}

	private void Kill()
	{
		if (OnCrash != null)
		{
			OnCrash();
		}
		OnCrash = null;
		m_active = false;
		m_destroyedAudioEvent.Play(m_audioSource, AudioController.Volume);
		if (!m_crashPS)
		{
			Debug.Log("here");
		}
		m_crashPS.gameObject.SetActive(true);
		m_spriteRenderer.enabled = false;
		if (OnDestroyed != null)
		{
			OnDestroyed(base.gameObject);
		}
		GetComponentInChildren<Collider2D>().enabled = false;
		GetComponentInChildren<SpriteRenderer>().enabled = false;
		GetComponent<Rigidbody2D>().isKinematic = true;
		StartCoroutine("DestroyAfterSeconds", 3);
	}

	private IEnumerator DestroyAfterSeconds(float duration)
	{
		float timer = 0f;
		while (timer < duration)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	public void ApplyForce(float force, ProjectileController projectile = null)
	{
		if (!m_active)
		{
			return;
		}
		CancelKinematic();
		if (force < m_endurance)
		{
			if ((bool)projectile && (bool)m_hitAudioEvent)
			{
				m_hitAudioEvent.Play(m_audioSource, AudioController.Volume);
			}
			return;
		}
		int num = (int)(force / m_endurance);
		m_currentSpriteIndex += num;
		if (m_currentSpriteIndex >= m_damagedSprites.Length)
		{
			Kill();
			return;
		}
		m_hitAudioEvent.Play(m_audioSource, AudioController.Volume);
		m_spriteRenderer.sprite = m_damagedSprites[m_currentSpriteIndex];
	}
}
