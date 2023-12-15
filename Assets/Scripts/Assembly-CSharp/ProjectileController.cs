using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileController : MonoBehaviour
{
	public delegate void StateEvent(State state);

	public enum State
	{
		Idle = 0,
		Equipped = 1,
		Airborne = 2,
		Crashed = 3,
		OnDestroy = 4
	}

	public UnityEvent OnStop;

	public UnityEvent OnCrash;

	public StateEvent OnStateChanged;

	public static int ANIMATOR_STATE_EQUIPPED = Animator.StringToHash("Equipped");

	public static int ANIMATOR_STATE_AIRBORNE = Animator.StringToHash("Airborne");

	public static int ANIMATOR_STATE_CRASHED = Animator.StringToHash("Crashed");

	public static int ANIMATOR_STATE_IDLE = Animator.StringToHash("IdleState");

	public static int ANIMATOR_STATE_IDLE_ANIM = Animator.StringToHash("IdleAnim");

	private State m_currentState;

	[SerializeField]
	private int m_idleAnimationsCount = 1;

	[SerializeField]
	private Vector2 m_idleAnimationsCooldown;

	[SerializeField]
	private float m_velocityForStop = 0.1f;

	[SerializeField]
	private ParticleSystem m_crashPS;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private Animator m_aspectAnimator;

	[SerializeField]
	private Breakable.Type[] m_strongAgainst;

	[SerializeField]
	private float[] m_strongAgainstMultiplier;

	[SerializeField]
	private Vector2 m_idleEventInterval;

	[SerializeField]
	private IAudioEvent m_idleEvent;

	[SerializeField]
	private IAudioEvent m_equippedEvent;

	[SerializeField]
	private IAudioEvent m_airborneEvent;

	[SerializeField]
	private IAudioEvent m_crashedEvent;

	[SerializeField]
	private IAudioEvent m_disabledEvent;

	[SerializeField]
	private LayerMask m_ground;

	[SerializeField]
	private float m_groundedDistance;

	private AudioSource m_audioSource;

	private SpriteRenderer m_renderer;

	private PuffTrail m_puffTrail;

	private Rigidbody2D m_rigidbody;

	private float m_idleCounter;

	private float m_currentIdleTimer = 4f;

	private bool m_crashed;

	private float m_stillTimer;

	private SpriteRenderer Renderer
	{
		get
		{
			if (!m_renderer)
			{
				m_renderer = GetComponentInChildren<SpriteRenderer>();
			}
			return m_renderer;
		}
	}

	private PuffTrail Trail
	{
		get
		{
			if (!m_puffTrail)
			{
				m_puffTrail = GetComponentInChildren<PuffTrail>();
			}
			return m_puffTrail;
		}
	}

	private Rigidbody2D Rigidbody
	{
		get
		{
			if (!m_rigidbody)
			{
				m_rigidbody = GetComponentInChildren<Rigidbody2D>();
			}
			return m_rigidbody;
		}
	}

	public State CurrentState
	{
		get
		{
			return m_currentState;
		}
		set
		{
			m_animator.enabled = true;
			m_aspectAnimator.enabled = true;
			switch (value)
			{
			case State.Idle:
				m_animator.SetBool(ANIMATOR_STATE_AIRBORNE, false);
				m_animator.SetBool(ANIMATOR_STATE_EQUIPPED, false);
				break;
			case State.Equipped:
				m_animator.SetBool(ANIMATOR_STATE_AIRBORNE, false);
				m_animator.SetBool(ANIMATOR_STATE_EQUIPPED, true);
				m_aspectAnimator.SetTrigger(ANIMATOR_STATE_EQUIPPED);
				break;
			case State.Airborne:
				Trail.enabled = true;
				m_animator.SetBool(ANIMATOR_STATE_AIRBORNE, true);
				m_animator.SetBool(ANIMATOR_STATE_EQUIPPED, false);
				m_aspectAnimator.SetTrigger(ANIMATOR_STATE_AIRBORNE);
				m_airborneEvent.Play(m_audioSource, AudioController.Volume);
				break;
			case State.Crashed:
				Trail.enabled = false;
				OnCrash.Invoke();
				m_animator.SetBool(ANIMATOR_STATE_AIRBORNE, false);
				m_aspectAnimator.SetTrigger(ANIMATOR_STATE_CRASHED);
				break;
			case State.OnDestroy:
				Dismiss(2f);
				break;
			}
			m_currentState = value;
			if (OnStateChanged != null)
			{
				OnStateChanged(value);
			}
		}
	}

	private void Awake()
	{
		m_currentIdleTimer = Random.Range(m_idleAnimationsCooldown.x, m_idleAnimationsCooldown.y);
		CurrentState = State.Idle;
		m_renderer = GetComponentInChildren<SpriteRenderer>();
		m_audioSource = GetComponentInChildren<AudioSource>();
		m_aspectAnimator.Play("Idle");
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
		switch (m_currentState)
		{
		case State.Idle:
			HandleIdle();
			break;
		case State.Equipped:
			HandleEquipped();
			break;
		case State.Airborne:
			HandleAirborne();
			break;
		case State.Crashed:
			HandleCrashed();
			break;
		}
	}

	private void HandleIdle()
	{
		m_idleCounter += Time.deltaTime;
		if (m_idleCounter >= m_currentIdleTimer)
		{
			m_idleCounter = 0f;
			m_animator.SetInteger(ANIMATOR_STATE_IDLE, Random.Range(0, m_idleAnimationsCount));
			m_animator.SetTrigger(ANIMATOR_STATE_IDLE_ANIM);
			m_idleEvent.Play(m_audioSource, AudioController.Volume);
			m_currentIdleTimer = Random.Range(m_idleAnimationsCooldown.x, m_idleAnimationsCooldown.y);
		}
	}

	private void HandleAirborne()
	{
	}

	private void HandleEquipped()
	{
	}

	public float GetDamageMultiplier(Breakable.Type type)
	{
		for (int i = 0; i < m_strongAgainst.Length; i++)
		{
			if (m_strongAgainst[i] == type)
			{
				return m_strongAgainstMultiplier[i];
			}
		}
		return 1f;
	}

	public void Dismiss(float delay)
	{
		StartCoroutine("Destroy", delay);
	}

	private IEnumerator Destroy(float delay)
	{
		Rigidbody.velocity *= 0f;
		yield return new WaitForSeconds(0.2f);
		m_crashPS.gameObject.SetActive(false);
		m_crashPS.gameObject.SetActive(true);
		GetComponentInChildren<SpriteRenderer>().enabled = false;
		GetComponentInChildren<Collider2D>().enabled = false;
		m_disabledEvent.Play(m_audioSource, AudioController.Volume);
		Rigidbody.isKinematic = true;
		Trail.System.transform.SetParent(GameObject.FindGameObjectWithTag("LevelParent").transform);
		yield return new WaitForSeconds(delay);
		OnStop.Invoke();
		OnCrash.Invoke();
		Object.Destroy(base.gameObject);
	}

	private void HandleCrashed()
	{
		RaycastHit2D[] array = Physics2D.RaycastAll(base.transform.position, -Vector2.up, 100f, m_ground);
		float num = 100f;
		for (int i = 0; i < array.Length; i++)
		{
			float num2 = base.transform.position.y - array[i].point.y;
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (m_velocityForStop > Rigidbody.velocity.magnitude)
		{
			m_stillTimer += Time.deltaTime;
		}
		else
		{
			m_stillTimer = 0f;
		}
		if (m_stillTimer > 0.2f && num < m_groundedDistance)
		{
			CurrentState = State.OnDestroy;
		}
		else if (m_stillTimer > 3f)
		{
			CurrentState = State.OnDestroy;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!m_crashed)
		{
			if (CurrentState != State.Crashed)
			{
				CurrentState = State.Crashed;
			}
			m_crashed = true;
		}
	}
}
