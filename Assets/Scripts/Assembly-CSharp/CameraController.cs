using System.Collections;
using Lean.Touch;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public delegate void CameraEvent();

	public enum State
	{
		AtStart = 0,
		FollowInput = 1,
		FollowTarget = 2,
		Wait = 3
	}

	[SerializeField]
	private State m_State;

	[SerializeField]
	private Transform m_Target;

	[SerializeField]
	private Transform m_TargetIdle;

	[SerializeField]
	private LeanCameraMoveSmooth m_cameraMove;

	[SerializeField]
	private LeanCameraZoomSmooth m_cameraZoom;

	[SerializeField]
	private float m_OffsetX;

	[SerializeField]
	private float m_Speed = 10f;

	[SerializeField]
	private Collider2D m_slingshotCollider;

	[SerializeField]
	private SlingshotController m_slingshot;

	private Camera m_camera;

	private bool m_preparingShot;

	private CameraShake m_shake;

	private int m_touchCounter;

	public Transform Target
	{
		get
		{
			return m_Target;
		}
		set
		{
			m_Target = value;
		}
	}

	public State CurrentState
	{
		get
		{
			return m_State;
		}
		set
		{
			m_State = value;
			m_cameraMove.enabled = m_State == State.FollowInput && !m_preparingShot;
			if (m_State == State.FollowInput && m_preparingShot)
			{
				m_slingshot.Cancel();
			}
			if (m_State == State.FollowInput)
			{
				m_touchCounter = 0;
			}
		}
	}

	public Vector3 StartPosition
	{
		get
		{
			return m_TargetIdle.position;
		}
	}

	public Vector3 LeftLimit
	{
		get
		{
			return m_cameraMove.Clamp.x * Vector3.right;
		}
	}

	private void OnEnable()
	{
		SetCameraInput(CurrentState == State.FollowInput);
	}

	private void OnDisable()
	{
		SetCameraInput(false);
	}

	private void To(Transform target, float offset, float speed)
	{
		Vector3 position = base.transform.position;
		position.x = Mathf.Lerp(position.x, target.position.x + offset, speed * Time.smoothDeltaTime);
		position.x = Mathf.Clamp(position.x, m_cameraMove.Clamp.x, m_cameraMove.Clamp.y);
		base.transform.position = position;
	}

	private IEnumerator MoveTo(Vector3 targetPos, CameraEvent onReached)
	{
		targetPos.y = base.transform.position.y;
		targetPos.z = base.transform.position.z;
		CurrentState = State.Wait;
		while (Mathf.Abs(targetPos.x - base.transform.position.x) > 0.1f)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, targetPos, Time.smoothDeltaTime * m_Speed);
			yield return null;
		}
		CurrentState = State.FollowInput;
		onReached();
	}

	public void Reset()
	{
		base.transform.position = new Vector3(m_TargetIdle.position.x, base.transform.position.y, base.transform.position.z);
		m_shake.Stop();
	}

	public void SetTarget(Transform target)
	{
		Target = target;
	}

	public void SetFollowTarget()
	{
		CurrentState = State.FollowTarget;
	}

	public void ToIdle()
	{
		m_Target = null;
		CurrentState = State.AtStart;
	}

	public void SetState(State state)
	{
		m_State = state;
	}

	public void To(Vector3 targetPos, CameraEvent onReached)
	{
		StartCoroutine(MoveTo(targetPos, onReached));
	}

	private void Awake()
	{
		m_shake = GetComponentInChildren<CameraShake>();
		m_camera = GetComponentInChildren<Camera>();
	}

	public void SetCameraInput(bool state)
	{
		m_cameraMove.enabled = state;
	}

	public void Shake(float duration, float force)
	{
		CurrentState = State.Wait;
		m_shake.Shake(duration, force);
		SetStateAfter(State.FollowInput, duration);
	}

	private IEnumerator SetStateAfter(State state, float seconds)
	{
		float timer = 0f;
		while (timer < seconds)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		CurrentState = state;
	}

	private void MouseDown()
	{
		RaycastHit2D[] rayIntersectionAll = Physics2D.GetRayIntersectionAll(m_camera.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity);
		if (rayIntersectionAll != null)
		{
			for (int i = 0; i < rayIntersectionAll.Length; i++)
			{
				if (rayIntersectionAll[i].collider == m_slingshotCollider)
				{
					if (!m_slingshot.Projectile)
					{
						break;
					}
					m_preparingShot = true;
					m_slingshot.Aim();
					SetCameraInput(false);
					return;
				}
			}
			SetCameraInput(true);
		}
		else
		{
			SetCameraInput(true);
		}
	}

	private void MouseUp()
	{
		if (m_preparingShot)
		{
			m_slingshot.Shoot();
		}
		m_preparingShot = false;
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.AtStart:
			if (TouchInput.TouchDown())
			{
				CurrentState = State.FollowInput;
				MouseDown();
			}
			else if (Mathf.Abs(m_TargetIdle.transform.position.x - base.transform.position.x) > 0.1f)
			{
				To(m_TargetIdle, 0f, m_Speed);
			}
			else
			{
				CurrentState = State.FollowInput;
			}
			break;
		case State.FollowInput:
			if (TouchInput.TouchDown())
			{
				MouseDown();
			}
			if (TouchInput.TouchUp())
			{
				MouseUp();
			}
			break;
		case State.FollowTarget:
			if (TouchInput.TouchDown())
			{
				m_touchCounter++;
				if (m_touchCounter < 2)
				{
					break;
				}
				CurrentState = State.FollowInput;
				MouseDown();
			}
			if ((bool)m_Target)
			{
				To(m_Target, m_OffsetX, m_Speed);
			}
			break;
		case State.Wait:
			break;
		}
	}
}
