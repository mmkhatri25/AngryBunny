using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	public UnityEvent OnObjectCrash;

	public UnityEvent OnExtraProjectileStart;

	public UnityEvent OnEarthquakeStart;

	public UnityEvent OnEarthquakeEnd;

	public UnityEvent OnRearm;

	[SerializeField]
	private List<Projectile> m_projectileInfo;

	[SerializeField]
	private SlingshotController m_slingshot;

	[SerializeField]
	private Transform m_projecticlesParent;

	[Header("Rearm params")]
	[SerializeField]
	private AnimationCurve m_rearmHeightCurve;

	[SerializeField]
	private float m_rearmHeight = 6f;

	[SerializeField]
	private AnimationCurve m_idlesHopHeightCurve;

	[SerializeField]
	private AnimationCurve m_idlesHopPositionCurve;

	[SerializeField]
	private float m_idlesHopHeight = 6f;

	[SerializeField]
	private float m_idlePositionStart = 1f;

	[SerializeField]
	private float m_idleDistance = -1f;

	[SerializeField]
	private float m_rearmDelay = 2f;

	[SerializeField]
	private float m_rearmDuration = 1f;

	private List<GameObject> m_projectiles;

	private GameObject m_projectileBeingEquipped;

	private int m_projectilesCount;

	private CameraController m_camera;

	private Earthquake m_Earthquake;

	private CameraController Camera
	{
		get
		{
			if (!m_camera)
			{
				m_camera = UnityEngine.Object.FindObjectOfType<CameraController>();
			}
			return m_camera;
		}
	}

	public List<GameObject> Projecticles
	{
		get
		{
			if (m_projectiles == null)
			{
				m_projectiles = new List<GameObject>();
			}
			return m_projectiles;
		}
		set
		{
			m_projectiles = value;
			Utils.ClearChildren(m_projecticlesParent);
			for (int i = 0; i < m_projectiles.Count; i++)
			{
				AddProjectile(m_projectiles[i], i);
			}
		}
	}

	public GameObject ProjectileBeingEquipped
	{
		get
		{
			return m_projectileBeingEquipped;
		}
	}

	public Transform ProjectilesParent
	{
		get
		{
			return m_projecticlesParent;
		}
	}

	private GameObject GetProjectile(Projectile.ProjectileType type)
	{
		for (int i = 0; i < m_projectileInfo.Count; i++)
		{
			if (m_projectileInfo[i].type == type)
			{
				return m_projectileInfo[i].prefab;
			}
		}
		throw new Exception("Couldn't find projectile " + type);
	}

	private void Awake()
	{
		m_Earthquake = GetComponentInChildren<Earthquake>();
	}

	private IEnumerator RearmAfterSecondsCoroutine(float seconds)
	{
		float timer = 0f;
		while (timer < seconds)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		Rearm();
	}

	private void Rearm()
	{
		EquipNextProjectile();
	}

	public void Init()
	{
		m_Earthquake.Init();
	}

	public void Set(List<Projectile.ProjectileType> list)
	{
		Utils.ClearChildren(m_projecticlesParent);
		m_projectiles = new List<GameObject>();
		m_projectilesCount = 0;
		ProjectileController[] array = UnityEngine.Object.FindObjectsOfType<ProjectileController>();
		m_Earthquake.Reset();
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i].gameObject);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			AddProjectile(GetProjectile(list[j]));
		}
	}

	public void AddProjectile(GameObject projectilePrefab, int index = -1)
	{
		float num = ((index != -1) ? index : Projecticles.Count);
		m_projectilesCount++;
		GameObject gameObject = UnityEngine.Object.Instantiate(projectilePrefab, m_projecticlesParent);
		ProjectileController componentInChildren = gameObject.GetComponentInChildren<ProjectileController>();
		componentInChildren.OnCrash.AddListener(ObjectCrash);
		Vector3 position = gameObject.transform.position;
		position.x = m_idlePositionStart + num * m_idleDistance;
		gameObject.transform.position = position;
		Projecticles.Add(gameObject);
	}

	public GameObject RemoveProjectile(int index)
	{
		GameObject result = Projecticles[index];
		Projecticles.RemoveAt(index);
		if (index == Projecticles.Count - 1)
		{
			return result;
		}
		return result;
	}

	public void EquipNextProjectile()
	{
		if (Projecticles.Count == 0)
		{
			Debug.Log("No more projectiles!");
			return;
		}
		m_slingshot.ResetDelayed(m_rearmDuration);
		StartCoroutine("EquipProjectile", RemoveProjectile(0));
	}

	private IEnumerator EquipProjectile(GameObject projectile)
	{
		float timer = 0f;
		m_projectileBeingEquipped = projectile;
		Vector3 initPos = projectile.transform.position;
		Vector3 startEuler = projectile.transform.eulerAngles;
		bool idlesMoved = false;
		while (timer < m_rearmDuration)
		{
			timer += Time.deltaTime;
			float rate = timer / m_rearmDuration;
			if (rate > 0.5f && !idlesMoved)
			{
				idlesMoved = true;
				StartCoroutine("MoveIdles");
			}
			Vector3 pos = Vector3.Lerp(initPos, m_slingshot.InitSlingshotPosition, rate);
			pos.y = Mathf.Lerp(pos.y, m_rearmHeight, m_rearmHeightCurve.Evaluate(rate));
			projectile.transform.position = pos;
			Vector3 euler = projectile.transform.eulerAngles;
			euler.z = startEuler.z + Mathf.Lerp(0f, 720f, rate);
			projectile.transform.eulerAngles = euler;
			yield return null;
		}
		projectile.transform.eulerAngles = startEuler;
		projectile.GetComponentInChildren<ProjectileController>().CurrentState = ProjectileController.State.Equipped;
		m_slingshot.Projectile = projectile.GetComponentInChildren<Rigidbody2D>();
		m_projectileBeingEquipped = null;
	}

	private IEnumerator MoveIdles()
	{
		for (int i = 0; i < m_projectiles.Count; i++)
		{
			StartCoroutine("MoveIdle", m_projectiles[i].transform);
			float timer = 0f;
			while (timer < 0.15f)
			{
				timer += Time.deltaTime;
				yield return null;
			}
		}
	}

	private IEnumerator MoveIdle(Transform _t)
	{
		Vector3 _startPos = _t.position;
		Vector3 _targetPos = _startPos - m_idleDistance * Vector3.right;
		float duration = 0.3f;
		float timer = 0f;
		while (timer < duration)
		{
			timer += Time.deltaTime;
			float rate = timer / duration;
			Vector3 pos = Vector3.Lerp(_startPos, _targetPos, m_idlesHopPositionCurve.Evaluate(rate));
			pos.y += m_rearmHeightCurve.Evaluate(rate) * m_idlesHopHeight;
			_t.position = pos;
			yield return null;
		}
	}

	public void ObjectCrash()
	{
		if (OnObjectCrash != null)
		{
			OnObjectCrash.Invoke();
		}
	}

	public void RearmAfterSeconds()
	{
		StartCoroutine("RearmAfterSecondsCoroutine", m_rearmDelay);
	}

	public void Reset()
	{
		StopAllCoroutines();
		m_Earthquake.StopAllCoroutines();
	}

	public void AddRandomProjectile()
	{
		if (GameStats.ExtraProjectilesCount > 0)
		{
			OnExtraProjectileStart.Invoke();
			RemoveExtraProjectile();
			Camera.To(Camera.LeftLimit, GiveRandomProjectile);
		}
	}

	private void GiveRandomProjectile()
	{
		AudioController.PlaySFX(1);
		AddProjectile(GetProjectile((Projectile.ProjectileType)UnityEngine.Random.Range(0, 3)));
	}

	public void Earthquake()
	{
		if (GameStats.QuakesCount > 0 && !m_Earthquake.Active)
		{
			List<Breakable> breakables = m_Earthquake.GetBreakables();
			if (breakables.Count != 0 && UnityEngine.Object.FindObjectOfType<GameplayController>().Playing)
			{
				OnEarthquakeStart.Invoke();
				RemoveEarthquake();
				Camera.To(Utils.GetCenter(breakables[0].transform.parent.parent), PerformEarthquake);
			}
		}
	}

	private void PerformEarthquake()
	{
		m_Earthquake.Apply(delegate
		{
			OnEarthquakeEnd.Invoke();
		});
	}

	public void AddExtraProjectile()
	{
		GameStats.ExtraProjectilesCount++;
		UnityEngine.Object.FindObjectOfType<ExtraProjectilesButton>().Refresh();
	}

	public void AddEarthquake()
	{
		GameStats.QuakesCount++;
		UnityEngine.Object.FindObjectOfType<EarthquakeButton>().Refresh();
	}

	private void RemoveExtraProjectile()
	{
		GameStats.ExtraProjectilesCount--;
		UnityEngine.Object.FindObjectOfType<ExtraProjectilesButton>().Refresh();
	}

	public void RemoveEarthquake()
	{
		GameStats.QuakesCount--;
		UnityEngine.Object.FindObjectOfType<EarthquakeButton>().Refresh();
	}
}
