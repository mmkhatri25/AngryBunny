using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	public MapAspect.Type theme;

	public int id;

	public int packID;

	public List<Projectile.ProjectileType> projectileTypes;

	private const string SAVE_KEY = "K@@Z<XXxAACVV.AA";

	private const string SCORE_KEY = "ZaZAASCRA";

	private const string STARS_KEY = "ss1ZSvCRA";

	public string SaveKey
	{
		get
		{
			return "K@@Z<XXxAACVV.AA" + id + packID;
		}
	}

	public string ScoreSaveKey
	{
		get
		{
			return SaveKey + "ZaZAASCRA";
		}
	}

	public string StarsSaveKey
	{
		get
		{
			return SaveKey + "ss1ZSvCRA";
		}
	}

	public int MaxScore
	{
		get
		{
			return SaveManager.LoadInt(ScoreSaveKey);
		}
	}

	public int MaxStars
	{
		get
		{
			return SaveManager.LoadInt(StarsSaveKey);
		}
	}

	public bool Locked
	{
		get
		{
			return SaveManager.LoadInt(SaveKey) == 0 && id != 0;
		}
		set
		{
			SaveManager.SaveInt(SaveKey, (!value) ? 1 : 0);
		}
	}

	public void SaveMaxScore(int score)
	{
		if (score > MaxScore)
		{
			SaveManager.SaveInt(ScoreSaveKey, score);
		}
	}

	public void SaveMaxStars(int count)
	{
		if (count > MaxStars)
		{
			SaveManager.SaveInt(StarsSaveKey, count);
			print("here is save stars - "+ StarsSaveKey + ", count is "+ count);
		}

	}

	public void Reload()
	{
		Target[] componentsInChildren = GetComponentsInChildren<Target>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].UpdateReference();
		}
	}
}
