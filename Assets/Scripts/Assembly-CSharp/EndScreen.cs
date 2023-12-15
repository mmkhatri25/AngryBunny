using System;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
	[SerializeField]
	private ScoreCountUp m_scoreDisplay;

	[SerializeField]
	private UIStarGroupAnimated m_scoreStars;

	public Text EndScreenLevel;
	public Text Message;
    public static Action<int> SetCurrentLevelAction;

    private void Awake()
    {
        SetCurrentLevelAction += SetCurrentlevel;
    }

    public void Set(int score, int stars)
	{
		m_scoreDisplay.Set(score);
		m_scoreStars.SetStatus(stars);
        SetCurrentlevel(PlayerPrefs.GetInt("m_id"));
	}
    public void SetCurrentlevel(int level)
    {
        print("here set level");
        EndScreenLevel.text = "Level " + level;
        if(level == 20)
        {
            Message.gameObject.SetActive(true);
        }
        else
        {
            Message.gameObject.SetActive(false);

        }

    }
}
