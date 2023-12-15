using System;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
	public delegate void StartLevelEvent(int packID, int levelID);

	public StartLevelEvent OnStartLevel;

	private LevelController m_levelController;

	[SerializeField]
	private GameObject m_uiLevelPrefab;

	[SerializeField]
	private Transform m_holder;

	private int m_packID;

	private LevelController LevelController
	{
		get
		{
			if (!m_levelController)
			{
				m_levelController = UnityEngine.Object.FindObjectOfType<LevelController>();
			}
			return m_levelController;
		}
	}

	public void Populate(int packID)
	{
		m_packID = packID;
		Utils.ClearChildren(m_holder);
        //for (int i = 0; i < LevelController.Packs[packID].Levels.Count; i++)
        for (int i = 0; i < 20; i++)
        {
            Level level = LevelController.Packs[packID].Levels[i];
			GameObject gameObject = UnityEngine.Object.Instantiate(m_uiLevelPrefab, m_holder);
			UILevel componentInChildren = gameObject.GetComponentInChildren<UILevel>();
			if (level.Locked)
			{
				componentInChildren.SetLocked(i);
				continue;
			}
			componentInChildren.OnSelected = (UILevel.SelectionEvent)Delegate.Combine(componentInChildren.OnSelected, new UILevel.SelectionEvent(StartLevel));
			componentInChildren.SetUnlocked(i, level.MaxStars);
			
			print("i "+ i + "max stars - "+ level.MaxStars);

		}
	}

	public void StartLevel(int levelID)
	{
		if (OnStartLevel != null)
		{
			OnStartLevel(m_packID, levelID);
		}
	}
}
