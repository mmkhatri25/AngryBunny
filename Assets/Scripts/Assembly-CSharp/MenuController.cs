using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
	public delegate void MenuEvent(MenuID id);

	public enum MenuID
	{
		Main = 0,
		PackSelect = 1,
		LevelSelect = 2,
		Gameplay = 3,
		EndWon = 4,
		Pause = 5,
		EndLost = 6
	}

	[Serializable]
	public struct MenuInfo
	{
		public GameObject reference;

		public bool hasBackground;
	}

	public MenuEvent OnMenuSet;

	public MenuEvent OnMenuLeft;

	public MenuEvent OnBack;

	[SerializeField]
	private MenuInfo[] m_Menus;

	[SerializeField]
	private GameObject m_Background;

	private List<MenuID> m_menuHistory;

	private MenuID m_currentMenu;

	public MenuID CurrentMenu
	{
		get
		{
			return m_currentMenu;
		}
	}

	public MenuID PreviousMenu
	{
		get
		{
			if (m_menuHistory.Count > 0)
			{
				return m_menuHistory[m_menuHistory.Count - 1];
			}
			throw new Exception("No previous menu!");
		}
	}

	public void SetMenu(MenuID id, bool addToHistory = true, bool forced = false)
	{
		if (m_currentMenu == id && !forced)
		{
			return;
		}
		if (OnMenuLeft != null)
		{
			OnMenuLeft(m_currentMenu);
		}
		m_Background.SetActive(m_Menus[(int)id].hasBackground);
		if (addToHistory)
		{
			if (m_menuHistory == null)
			{
				m_menuHistory = new List<MenuID>();
			}
			else
			{
				m_menuHistory.Add(m_currentMenu);
			}
		}
		m_currentMenu = id;
		for (int i = 0; i < m_Menus.Length; i++)
		{
			m_Menus[i].reference.SetActive(i == (int)id);
		}
		if (OnMenuSet != null)
		{
			OnMenuSet(id);
		}
	}

	public void Back()
	{
		//m_menuHistory = null;

        if (m_menuHistory != null && m_menuHistory.Count != 0)
		{
			SetMenu(m_menuHistory[m_menuHistory.Count - 1], false);
			m_menuHistory.RemoveAt(m_menuHistory.Count - 1);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && OnBack != null)
		{
			OnBack(m_currentMenu);
		}
	}
}
