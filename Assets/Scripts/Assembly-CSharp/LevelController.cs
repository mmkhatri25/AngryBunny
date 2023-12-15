using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	[Serializable]
	public class Pack
	{
		[SerializeField]
		private string m_name = "Pack";

		[SerializeField]
		private List<Level> m_levels;

		[SerializeField]
		private bool m_locked;

		public List<Level> Levels
		{
			get
			{
				if (m_levels == null)
				{
					m_levels = new List<Level>();
				}
				return m_levels;
			}
			set
			{
				m_levels = value;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public bool Locked
		{
			get
			{
				return m_locked;
			}
			set
			{
				m_locked = value;
			}
		}
	}

	[SerializeField]
	private Transform m_levelsParent;

	[SerializeField]
	private List<Pack> m_packs;

	private MapAspect m_mapAspect;

	private PlayerController m_playerController;

	private SlingshotController m_slingshot;

	private CameraController m_cameraController;

	[SerializeField]
	private Level m_currentLevel;

	private int m_currentLevelMaxScore;

	[SerializeField]
	private string m_savePath;

	private static LevelController m_instance;

	private static LevelController Instance
	{
		get
		{
			if (!m_instance)
			{
				m_instance = UnityEngine.Object.FindObjectOfType<LevelController>();
			}
			return m_instance;
		}
	}

	public static int LevelsUnlockedCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < Instance.Packs.Count; i++)
			{
				for (int j = 0; j < Instance.Packs[i].Levels.Count; j++)
				{
					if (!Instance.Packs[i].Levels[i].Locked)
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	private MapAspect LevelAspect
	{
		get
		{
			if (!m_mapAspect)
			{
				m_mapAspect = UnityEngine.Object.FindObjectOfType<MapAspect>();
			}
			return m_mapAspect;
		}
	}

	private CameraController Camera
	{
		get
		{
			if (!m_cameraController)
			{
				m_cameraController = UnityEngine.Object.FindObjectOfType<CameraController>();
			}
			return m_cameraController;
		}
	}

	public Level CurrentLevel
	{
		get
		{
			return m_currentLevel;
		}
	}

	public int CurrentLevelMaxScore
	{
		get
		{
			return m_currentLevelMaxScore;
		}
	}

	public SlingshotController Slingshot
	{
		get
		{
			if (!m_slingshot)
			{
				m_slingshot = UnityEngine.Object.FindObjectOfType<SlingshotController>();
			}
			return m_slingshot;
		}
	}

	public PlayerController PlayerController
	{
		get
		{
			if (!m_playerController)
			{
				m_playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
			}
			return m_playerController;
		}
	}

	public List<Pack> Packs
	{
		get
		{
			return m_packs;
		}
		set
		{
			m_packs = value;
		}
	}

	public string SavePath
	{
		get
		{
			return m_savePath;
		}
	}

	public Transform LevelsParent
	{
		get
		{
			return m_levelsParent;
		}
	}

	public static void UnlockPack(int packID)
	{
		Instance.Packs[packID].Locked = false;
	}

	public static bool IsPackLocked(int packID)
	{
		return Instance.Packs[packID].Locked;
	}

	public void AddPack(Pack pack)
	{
		Packs.Add(pack);
	}

	public void AddLevel(int packID, Level level)
	{
		m_packs[packID].Levels.Add(level);
	}

	public void LoadLevel(int packID, int levelID)
	{
		LoadLevel(m_packs[packID].Levels[levelID]);
	}

	public void LoadLevel(Level level)
	{
		Utils.ClearChildren(m_levelsParent);
		m_currentLevel = UnityEngine.Object.Instantiate(level, m_levelsParent);
		m_currentLevel.transform.localPosition = Vector3.zero;
		PlayerController.Reset();
		PlayerController.Set(level.projectileTypes);
		PlayerController.EquipNextProjectile();
		StartCoroutine("WaitForStart");
		UnityEngine.Object.FindObjectOfType<CameraController>().Reset();
		Slingshot.Reset();
		LevelAspect.ApplyAspect(level.theme);
		Breakable[] array = UnityEngine.Object.FindObjectsOfType<Breakable>();
		foreach (Breakable obj in array)
		{
			obj.OnCrash = (Breakable.BreakableEvent)Delegate.Combine(obj.OnCrash, new Breakable.BreakableEvent(PlayerController.ObjectCrash));
		}
		m_currentLevelMaxScore = 0;
		StartCoroutine("CalculateMaxScore");
	}

	private IEnumerator CalculateMaxScore()
	{
		yield return new WaitForEndOfFrame();
		Score[] scores = m_levelsParent.GetComponentsInChildren<Score>();
		for (int i = 0; i < scores.Length; i++)
		{
			m_currentLevelMaxScore += scores[i].ScoreAmount;
		}
		ProjectileController[] projectiles = PlayerController.ProjectilesParent.GetComponentsInChildren<ProjectileController>();
		for (int j = 0; j < projectiles.Length - 1; j++)
		{
			m_currentLevelMaxScore += projectiles[j].GetComponentInChildren<Score>().ScoreAmount;
		}
		Debug.Log("Max Score is: " + m_currentLevelMaxScore);
		PlayerController.Init();
	}

	private IEnumerator WaitForStart()
	{
		Camera.SetState(CameraController.State.Wait);
		while ((bool)PlayerController.ProjectileBeingEquipped)
		{
			yield return null;
		}
		Camera.SetState(CameraController.State.FollowInput);
	}
}
