using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameplayController : MonoBehaviour
{
	[Serializable]
	public class ScoreEvent : UnityEvent<string>
	{
	}

	public ScoreEvent OnScoreModified;

	[SerializeField]
	private LevelController m_levelController;

	[SerializeField]
	private PlayerController m_player;

	[SerializeField]
	private SlingshotController m_slingshot;

	[SerializeField]
	private GameObject m_gameplayParent;

	[SerializeField]
	private int m_starsCount = 3;

	[SerializeField]
	private float m_starsScoreRate = 0.9f;

	[SerializeField]
	private float m_resetCooldown = 2f;

	[SerializeField]
	private CameraController m_camera;

	private static GameplayController m_instance;

	private int m_ObjectivesCount;

	private float m_timeSinceCrash;

	private int m_currentScore;

	private bool m_resetCamera;

	private bool m_nextProjectile;

	private bool m_playing;

	private Vector2Int m_currentLevel;

	public LevelController LevelController
	{
		get
		{
			return m_levelController;
		}
	}

	public int CurrentScore
	{
		get
		{
			return m_currentScore;
		}
	}

	public int CurrentStars
	{
		get
		{
			return (int)Mathf.Floor((float)m_currentScore / ((float)m_levelController.CurrentLevelMaxScore * m_starsScoreRate) * (float)m_starsCount);
		}
	}

	public bool ResetCameraState
	{
		get
		{
			return m_resetCamera;
		}
		set
		{
			m_resetCamera = value;
		}
	}

	private static GameplayController Instance
	{
		get
		{
			if (!m_instance)
			{
				m_instance = UnityEngine.Object.FindObjectOfType<GameplayController>();
			}
			return m_instance;
		}
	}

	public bool Playing
	{
		get
		{
			return m_playing;
		}
	}

	public Vector2Int CurrentLevel
	{
		get
		{
			return m_currentLevel;
		}
	}

	public static Level CurrentLevelReference
	{
		get
		{
			return Instance.LevelController.CurrentLevel;
		}
	}

	private void Awake()
	{
		m_slingshot.OnShoot.AddListener(delegate
		{
			StopCoroutine("WaitForReset");
			ResetCameraState = true;
		});
		m_player.OnEarthquakeStart.AddListener(delegate
		{
			ResetCameraState = false;
		});
		m_player.OnExtraProjectileStart.AddListener(delegate
		{
			ResetCameraState = false;
		});
		m_player.OnEarthquakeEnd.AddListener(delegate
		{
			ResetCameraState = true;
			ObjectCrash();
		});
	}

	private void SetScore(int amount)
	{
		m_currentScore = amount;
		OnScoreModified.Invoke(m_currentScore.ToString());
	}

	public void AddScore(int amount)
	{
		m_currentScore += amount;
		OnScoreModified.Invoke(m_currentScore.ToString());
	}

	private void AddScore(GameObject breakable)
	{
		Score component = breakable.GetComponent<Score>();
		SetScore(m_currentScore + component.ScoreAmount);
		component.Show(component.ScoreAmount);
	}

	private void ObjectiveProgress(GameObject breakable)
	{
		m_ObjectivesCount--;
	}

	private void ProcessProjectileScore(ProjectileController projectile)
	{
		Score componentInChildren = projectile.GetComponentInChildren<Score>();
		AddScore(componentInChildren.ScoreAmount);
		componentInChildren.Show(componentInChildren.ScoreAmount);
		projectile.Dismiss(3f);
	}

	private IEnumerator EndGame()
	{
		Debug.Log("EndGame");
		float delay = 0.5f;
		m_camera.To(m_camera.LeftLimit, delegate
		{
			m_nextProjectile = true;
		});
		while (!m_nextProjectile)
		{
			yield return null;
		}
		m_nextProjectile = false;
		if (m_player.ProjectileBeingEquipped != null)
		{
			ProcessProjectileScore(m_player.ProjectileBeingEquipped.GetComponentInChildren<ProjectileController>());
			yield return new WaitForSeconds(delay);
		}
		if (m_slingshot.Projectile != null)
		{
			m_camera.To(m_camera.LeftLimit, delegate
			{
				m_nextProjectile = true;
			});
			while (!m_nextProjectile)
			{
				yield return null;
			}
			m_nextProjectile = false;
			ProcessProjectileScore(m_slingshot.Projectile.GetComponentInChildren<ProjectileController>());
			yield return new WaitForSeconds(delay);
		}
		for (int i = 0; i < m_player.Projecticles.Count; i++)
		{
			m_camera.To(m_camera.LeftLimit, delegate
			{
				m_nextProjectile = true;
			});
			while (!m_nextProjectile)
			{
				yield return null;
			}
			m_nextProjectile = false;
			ProcessProjectileScore(m_player.Projecticles[i].GetComponentInChildren<ProjectileController>());
			yield return new WaitForSeconds(delay);
		}
		UnityEngine.Object.FindObjectOfType<GameController>().GameWon();
	}

	private bool CheckGameOver()
	{
		if (!m_playing && m_ObjectivesCount == 0)
		{
			return false;
		}
		if (m_ObjectivesCount == 0)
		{
			StopAllCoroutines();
			StopAllCoroutines();
			StartCoroutine("EndGame");
			ResetCameraState = false;
			m_playing = false;
			return true;
		}
		if (m_player.Projecticles.Count <= 0 && m_slingshot.Projectile == null && !m_player.ProjectileBeingEquipped)
		{
			UnityEngine.Object.FindObjectOfType<GameController>().GameLost();
			StopAllCoroutines();
			m_playing = false;
			return true;
		}
		return false;
	}

	public void ResetCamera()
	{
		if (m_resetCamera)
		{
			m_camera.ToIdle();
		}
	}

	private IEnumerator CountObjectives()
	{
		yield return new WaitForEndOfFrame();
		Breakable[] _breakables = m_levelController.CurrentLevel.GetComponentsInChildren<Breakable>();
		for (int i = 0; i < _breakables.Length; i++)
		{
			Breakable obj = _breakables[i];
			obj.OnDestroyed = (Breakable.BreakableObjectEvent)Delegate.Combine(obj.OnDestroyed, new Breakable.BreakableObjectEvent(AddScore));
			Breakable obj2 = _breakables[i];
			obj2.OnDestroyed = (Breakable.BreakableObjectEvent)Delegate.Combine(obj2.OnDestroyed, (Breakable.BreakableObjectEvent)delegate
			{
				ObjectCrash();
			});
			if (_breakables[i].Objective)
			{
				m_ObjectivesCount++;
				Breakable obj3 = _breakables[i];
				obj3.OnDestroyed = (Breakable.BreakableObjectEvent)Delegate.Combine(obj3.OnDestroyed, new Breakable.BreakableObjectEvent(ObjectiveProgress));
			}
		}
		Debug.Log("This level has " + m_ObjectivesCount + " objectives");
		if (m_ObjectivesCount == 0)
		{
			Debug.LogError("You have no objective targets for this level!!!");
		}
	}

	private IEnumerator WaitForReset()
	{
		while (m_timeSinceCrash < m_resetCooldown)
		{
			m_timeSinceCrash += Time.deltaTime;
			yield return null;
		}
		if (!CheckGameOver())
		{
			ResetCamera();
		}
	}

	public void ObjectCrash()
	{
		m_timeSinceCrash = 0f;
		StopCoroutine("WaitForReset");
		StartCoroutine("WaitForReset");
	}

	public void StartLevel(int packID, int levelID)
	{
		StopAllCoroutines();
		m_playing = true;
		m_currentLevel.x = packID;
		m_currentLevel.y = levelID;
		StopAllCoroutines();
		Show();
		m_levelController.LoadLevel(packID, levelID);
		m_ObjectivesCount = 0;
		SetScore(0);
		StartCoroutine("CountObjectives");
	}

	public void QuitGame()
	{
		StopAllCoroutines();
		m_playing = false;
		Hide();
	}

	public void Show()
	{
		m_gameplayParent.SetActive(true);
	}

	public void Hide()
	{
		m_gameplayParent.SetActive(false);
	}
}
