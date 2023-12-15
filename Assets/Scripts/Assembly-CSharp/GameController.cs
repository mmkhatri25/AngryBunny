using System;
using System.Collections;
using CaramelAds;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public enum GameOverReason
	{
		Lost = 0,
		Won = 1
	}
    public PlayerController playercntroler;
    public LevelController levelController;
	public GameObject SlingshotController;


    [SerializeField]
	private MenuController m_menuController;

	[SerializeField]
	private GameplayController m_gameplayController;

	[SerializeField]
	private LevelSelect m_levelSelect;

	[SerializeField]
	private EndScreen m_endScreen;

	[SerializeField]
	private PopupFactory m_popupFactory;

	[SerializeField]
	private int m_adAfterGames = 3;

	private bool m_gameOver;

	private CameraController m_cameraController;

	private bool m_playing;

	private bool m_vibrationEnabled = true;

	private int m_adCounter;

	private static GameController m_instance;
	public  Text CurrentLevel;
    public static Action<int> SetCurrentLevelAction;


    private static GameController Instance
	{
		get
		{
			if (!m_instance)
			{
				m_instance = UnityEngine.Object.FindObjectOfType<GameController>();
			}
			return m_instance;
		}
	}

	private void Awake()
	{
		Application.targetFrameRate = 60;
		m_cameraController = UnityEngine.Object.FindObjectOfType<CameraController>();
		m_cameraController.SetCameraInput(false);
		SetCurrentLevelAction += SetCurrentlevel;
	}

	private void Start()
	{
		LevelSelect levelSelect = m_levelSelect;
		levelSelect.OnStartLevel = (LevelSelect.StartLevelEvent)Delegate.Combine(levelSelect.OnStartLevel, new LevelSelect.StartLevelEvent(StartLevel));
		m_gameplayController.Hide();
		MenuController menuController = m_menuController;
		menuController.OnMenuSet = (MenuController.MenuEvent)Delegate.Combine(menuController.OnMenuSet, new MenuController.MenuEvent(HandleMenuSwitch));
		MenuController menuController2 = m_menuController;
		menuController2.OnMenuLeft = (MenuController.MenuEvent)Delegate.Combine(menuController2.OnMenuLeft, new MenuController.MenuEvent(HandleMenuleft));
		m_menuController.SetMenu(MenuController.MenuID.Main, true, true);
		PopupFactory popupFactory = m_popupFactory;
		popupFactory.OnPopupCreated = (PopupFactory.PopupEvent)Delegate.Combine(popupFactory.OnPopupCreated, (PopupFactory.PopupEvent)delegate
		{
			StopTime();
		});
		PopupFactory popupFactory2 = m_popupFactory;
		popupFactory2.OnPopupDestroyed = (PopupFactory.PopupEvent)Delegate.Combine(popupFactory2.OnPopupDestroyed, (PopupFactory.PopupEvent)delegate
		{
			ResumeTime();
		});
		AudioController.SetSourceVolume(3, 0f);
	}

	private void HandleMenuleft(MenuController.MenuID id)
	{
		switch (id)
		{
		case MenuController.MenuID.Main:
			break;
		case MenuController.MenuID.PackSelect:
			break;
		case MenuController.MenuID.LevelSelect:
			break;
		case MenuController.MenuID.Gameplay:
			AudioController.StopAmbientSource();
			m_cameraController.SetCameraInput(false);
			StopTime();
			break;
		case MenuController.MenuID.EndWon:
			break;
		case MenuController.MenuID.Pause:
			Time.timeScale = 1f;
			break;
		}
	}

	private void HandleMenuSwitch(MenuController.MenuID id)
	{
		switch (id)
		{
		case MenuController.MenuID.Main:
			AudioController.StartBackgroundSource();
			break;
		case MenuController.MenuID.PackSelect:
			break;
		case MenuController.MenuID.LevelSelect:
			break;
		case MenuController.MenuID.Gameplay:
			AudioController.StopBackgroundSource();
			AudioController.StartAmbientSource();
			m_cameraController.SetCameraInput(true);
			ResumeTime();
			break;
		case MenuController.MenuID.EndWon:
			AudioController.StopBackgroundSource();
			AudioController.StartBackgroundSource();
			break;
		case MenuController.MenuID.Pause:
			AudioController.StopBackgroundSource();
			AudioController.StartBackgroundSource();
			break;
		case MenuController.MenuID.EndLost:
			AudioController.StartBackgroundSource();
			break;
		}
	}

	private void UnlockNextLevel()
	{
		int x = m_gameplayController.CurrentLevel.x;
		int y = m_gameplayController.CurrentLevel.y;
		//print("UnlockNextLevel - "+ m_gameplayController.LevelController.Packs[x].Levels[y + 1].Locked);
		//if (m_gameplayController.LevelController.Packs[x].Levels.Count - 1 > y)
		{
			m_gameplayController.LevelController.Packs[x].Levels[y + 1].Locked = false;
           // print("UnlockNextLevel - " + m_gameplayController.LevelController.Packs[x].Levels[y + 1].Locked);

        }
        //if (IAPManager.instance.IsBought(IAPManager.StoreProductID.BunniesPerLevel))
        //{
        //	IAPCallbacks.AddBunnies(IAPManager.instance.GetProduct(IAPManager.StoreProductID.BunniesPerLevel).amount);
        //}
    }

	private void SaveScore()
	{
		//Debug.Log("Score: " + m_gameplayController.CurrentScore + "/ " + m_gameplayController.LevelController.CurrentLevelMaxScore + ", m_gameplayController.CurrentStars: " + m_gameplayController.CurrentStars);
		m_gameplayController.LevelController.CurrentLevel.SaveMaxScore(m_gameplayController.CurrentScore);
        int a = playercntroler.ProjectilesParent.transform.childCount; // remaining bunny
        int b = levelController.CurrentLevel.projectileTypes.Count; // total bunny for this level
		if (SlingshotController.transform.childCount == 2)
		{
			a += 1;
		}
		//if (b == 0)
		//	b = 0;
		//else
		//	b += 1;

		int count = 0;//m_gameplayController.CurrentStars;
        count = CalculateStarRating(b, a);
        Debug.Log("total bunny - " + b + " , remaining bunny -  " + a + "bunny on sling - "+ SlingshotController.transform.childCount/*+ " , name - "+ SlingshotController.transform.GetChild(1).gameObject.name*/);

        //m_gameplayController.LevelController.CurrentLevel.SaveMaxStars(count);

        //m_gameplayController.LevelController.CurrentLevel.SaveMaxStars(m_gameplayController.CurrentStars);
    }
    public int CalculateStarRating(int bunnyCount, int remainingBunnies)
    {
        int starRating = 0;

        switch (bunnyCount)
        {
            case 1:
                starRating = 3;
                break;

            case 2:
                if (remainingBunnies == 1) starRating = 3;
                else if (remainingBunnies == 0) starRating = 2;
                break;
            case 3:
                if (remainingBunnies == 0) starRating = 1;
                else if (remainingBunnies == 1) starRating = 2;
                else if (remainingBunnies == 2) starRating = 3;
                break;
            case 4:
                if (remainingBunnies == 2 || remainingBunnies == 3) starRating = 3;
                else if (remainingBunnies == 1) starRating = 2;
                else if (remainingBunnies == 0) starRating = 1;
                break;
            case 5:
                if (remainingBunnies == 3 || remainingBunnies == 4) starRating = 3;
                else if (remainingBunnies == 1 || remainingBunnies == 2) starRating = 2;
                else if (remainingBunnies == 0) starRating = 1;
                break;
            case 6:
                if (remainingBunnies == 5 || remainingBunnies == 4) starRating = 3;
                else if (remainingBunnies == 1 || remainingBunnies == 2) starRating = 2;
                else if (remainingBunnies == 0) starRating = 1;
                break;
            case 7:
                if (remainingBunnies == 6 || remainingBunnies == 5) starRating = 3;
                else if (remainingBunnies == 4 || remainingBunnies == 3 || remainingBunnies == 2) starRating = 2;
                else if (remainingBunnies == 1 || remainingBunnies == 0) starRating = 1;
                break;
            case 8:
                if (remainingBunnies == 7 || remainingBunnies == 6 || remainingBunnies == 5) starRating = 3;
                else if (remainingBunnies == 4 || remainingBunnies == 3 || remainingBunnies == 2) starRating = 2;
                else if (remainingBunnies == 1 || remainingBunnies == 0) starRating = 1;
                break;
        }
        Debug.Log("got final star rating  - "+ starRating);

        m_gameplayController.LevelController.CurrentLevel.SaveMaxStars(starRating);
		m_endScreen.Set(m_gameplayController.CurrentScore, starRating);

        return starRating;
    }

    private IEnumerator EndGame(GameOverReason reason)
	{
		m_playing = false;
		yield return new WaitForSeconds(1f);
		if (reason == GameOverReason.Won)
		{
			m_menuController.SetMenu(MenuController.MenuID.EndWon);
			//m_endScreen.Set(m_gameplayController.CurrentScore, m_gameplayController.CurrentStars);
		}
		else
		{
			m_menuController.SetMenu(MenuController.MenuID.EndLost);
		}
		//m_adCounter++;
		//if (m_adCounter >= m_adAfterGames)
		//{
		//	if (AdNetworksManager.instance.HasInterstitial())
		//	{
		//		AdNetworksManager.instance.ShowInterstitial(null);
		//		m_adCounter = 0;
		//	}
		//	else
		//	{
		//		CaramelNative.Cache();
		//	}
		//}
		if (reason != 0 && reason == GameOverReason.Won)
		{
			UnlockNextLevel();
			SaveScore();
		}
	}

	public void ToMainMenu()
	{
		m_menuController.SetMenu(MenuController.MenuID.Main);
        m_menuController.SetMenu(MenuController.MenuID.PackSelect);
    }

	public void ToPackSelect()
	{
		m_menuController.SetMenu(MenuController.MenuID.PackSelect);
	}

	public void ToNextLevel()
	{
		m_menuController.SetMenu(MenuController.MenuID.Main);
		m_menuController.SetMenu(MenuController.MenuID.PackSelect);
		m_menuController.SetMenu(MenuController.MenuID.LevelSelect);
		m_levelSelect.Populate(m_gameplayController.CurrentLevel.x);
	}

	public void ToLevelSelect()
	{
		m_menuController.SetMenu(MenuController.MenuID.LevelSelect);
	}

	public void StartLevel(int packID, int levelID)
	{
		StopAllCoroutines();
		m_playing = true;
		m_gameOver = false;
		Debug.Log("Starting level " + packID + " " + levelID);
		GameStats.LastPackPlayed = packID;
		GameStats.SetLastLevelPlayed(packID, levelID);
		m_gameplayController.StartLevel(packID, levelID);
		m_menuController.SetMenu(MenuController.MenuID.Gameplay);
	}

	public void GameOver(GameOverReason reason)
	{
		if (!m_gameOver)
		{
			m_gameOver = true;
			StartCoroutine(EndGame(reason));
		}
	}

	public void GameLost()
	{
		GameOver(GameOverReason.Lost);
	}

	public void GameWon()
	{
		GameOver(GameOverReason.Won);
	}

	public void RestartLevel()
	{
		StartLevel(GameStats.LastPackPlayed, GameStats.GetLastLevelPlayed(GameStats.LastPackPlayed));
	}

	public void Pause()
	{
		m_menuController.SetMenu(MenuController.MenuID.Pause);
		m_playing = false;
		StopTime();
	}

	public void QuitLevel()
	{
		m_gameplayController.QuitGame();
		ToMainMenu();
	}
	public void backToHome()
	{
        ToPackSelect();
    }

	public void Resume()
	{
		m_menuController.SetMenu(MenuController.MenuID.Gameplay);
		m_playing = true;
		ResumeTime();
	}

	public void StopTime()
	{
		Time.timeScale = 0f;
		Time.timeScale = 0f;
	}

	public void ResumeTime()
	{
		Time.timeScale = 1f;
		Time.timeScale = 1f;
	}

	public static void Vibrate()
	{
		if (Instance.m_vibrationEnabled)
		{
			Handheld.Vibrate();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
	}
	public void AddBunnies(UnityEngine.UI.Text text)
	{
        GameStats.ExtraProjectilesCount += 10;
		text.text = GameStats.ExtraProjectilesCount + "";
    }
    public void AddEarth(UnityEngine.UI.Text text)
    {
        GameStats.QuakesCount += 10;
        text.text = GameStats.QuakesCount + "";
    }
	public void SetCurrentlevel(int level)
	{
        CurrentLevel.text = "Level " + level;

    }
}
