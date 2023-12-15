using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SlingshotController : MonoBehaviour
{
	[Serializable]
	public class ProjectileEvent : UnityEvent<Transform>
	{
	}

	[Header("References")]
	[SerializeField]
	private Transform m_Sling;

	[SerializeField]
	private LineRenderer m_BandFront;

	[SerializeField]
	private LineRenderer m_BandBack;

	[SerializeField]
	private IAudioEvent m_AimEvent;

	[Header("Parameters")]
	[SerializeField]
	private float m_MaxStretch = 3f;

	[SerializeField]
	private float m_MinHeight;

	[SerializeField]
	private Vector3 m_projectileEquippedPos;

	[SerializeField]
	private float m_minForce = 10f;

	public ProjectileEvent OnShoot;

	private AudioSource m_audioSource;

	private Rigidbody2D m_SlingRigidbody;

	private SpringJoint2D m_spring;

	private Rigidbody2D m_Projectile;

	private SpringJoint2D m_Spring;

	private float m_MaxStretchSqr;

	private bool m_PreparingShot;

	private Ray m_RayToMouse;

	private Vector3 m_SlingVelocityCache;

	private Vector3 m_SlignInitPos;

	private float m_initSpringFrequency;

	private bool m_beingReset;

	public Rigidbody2D Projectile
	{
		get
		{
			return m_Projectile;
		}
		set
		{
			m_Projectile = value;
			m_Projectile.transform.SetParent(m_Sling);
			m_Projectile.transform.localPosition = m_projectileEquippedPos;
		}
	}

	public Vector3 InitSlingshotPosition
	{
		get
		{
			return m_SlignInitPos;
		}
	}

	private void Awake()
	{
		m_Spring = GetComponentInChildren<SpringJoint2D>();
		m_RayToMouse = new Ray(base.transform.position, Vector3.zero);
		m_MaxStretchSqr = m_MaxStretch * m_MaxStretch;
		m_SlingRigidbody = m_Sling.GetComponent<Rigidbody2D>();
		m_SlignInitPos = m_Sling.position;
		m_spring = m_Sling.GetComponentInChildren<SpringJoint2D>();
		m_initSpringFrequency = m_spring.frequency;
		m_audioSource = GetComponentInChildren<AudioSource>();
	}

	private void Start()
	{
		Setup();
	}

	private void Setup()
	{
		m_BandFront.SetPosition(0, Vector3.zero);
		m_BandBack.SetPosition(0, Vector3.zero);
	}

	public void Aim()
	{
		if (!m_beingReset)
		{
			m_Spring.enabled = false;
			m_PreparingShot = true;
			m_AimEvent.Play(m_audioSource, AudioController.Volume);
		}
	}

	public void Shoot()
	{
		if (!m_beingReset)
		{
			m_Spring.enabled = true;
			m_PreparingShot = false;
			m_SlingRigidbody.bodyType = RigidbodyType2D.Dynamic;
			m_AimEvent.Stop();
		}
	}

	public void Cancel()
	{
		m_Spring.enabled = false;
		m_PreparingShot = false;
		ResetDelayed(0.1f);
	}

	private void Drag()
	{
		if (!m_beingReset)
		{
			Vector3 zero = Vector3.zero;
			zero = Input.GetTouch(0).position;
			//zero = Input.mousePosition;
			Vector3 vector = Camera.main.ScreenToWorldPoint(zero);
			Vector2 vector2 = vector - base.transform.position;
			if (vector2.sqrMagnitude > m_MaxStretchSqr)
			{
				m_RayToMouse.direction = vector2;
				vector = m_RayToMouse.GetPoint(m_MaxStretch);
			}
			vector.z = 0f;
			vector.x = Mathf.Min(vector.x, base.transform.position.x);
			vector.y = Mathf.Max(m_MinHeight, vector.y);
			m_Sling.position = Vector3.Lerp(base.transform.position, vector, 50f * Time.smoothDeltaTime);
		}
	}

	private void UpdateBands()
	{
		m_BandBack.SetPosition(1, m_Sling.position - m_BandBack.transform.parent.position);
		m_BandFront.SetPosition(1, m_Sling.position - m_BandFront.transform.parent.position);
	}

	private void ReleaseSling()
	{
		if (!m_beingReset)
		{
			if (m_SlingVelocityCache.magnitude < m_minForce)
			{
				Cancel();
				return;
			}
			m_Projectile.transform.SetParent(null);
			m_Projectile.isKinematic = false;
			m_Projectile.velocity = m_SlingVelocityCache;
			m_Projectile.GetComponentInChildren<ProjectileController>().CurrentState = ProjectileController.State.Airborne;
			OnShoot.Invoke(Projectile.transform);
			m_Projectile = null;
			m_spring.frequency = 0f;
		}
	}

	private void Update()
	{
		if (m_PreparingShot)
		{
			Drag();
		}
		if ((bool)m_Projectile)
		{
			if (!m_SlingRigidbody.isKinematic && m_SlingVelocityCache.sqrMagnitude > m_SlingRigidbody.velocity.sqrMagnitude)
			{
				ReleaseSling();
			}
			else if (!m_PreparingShot)
			{
				m_SlingVelocityCache = m_SlingRigidbody.velocity;
			}
		}
	}

	private void LateUpdate()
	{
		UpdateBands();
	}

	private IEnumerator ResetDelayedCoroutine(float duration)
	{
		float timer = 0f;
		Vector3 startPos = m_Sling.position;
		m_beingReset = true;
		while (timer < duration)
		{
			timer += Time.deltaTime;
			m_Sling.position = Vector3.Lerp(startPos, m_SlignInitPos, timer / duration);
			yield return null;
		}
		Reset();
	}

	public void ResetDelayed(float delay)
	{
		StartCoroutine("ResetDelayedCoroutine", delay);
	}

	public void Reset()
	{
		m_SlingRigidbody.isKinematic = true;
		m_SlingRigidbody.velocity = Vector3.zero;
		m_Sling.position = m_SlignInitPos;
		m_spring.frequency = m_initSpringFrequency;
		m_beingReset = false;
	}
}
