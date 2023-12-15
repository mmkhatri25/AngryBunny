public static class GameStats
{
	private const string EXTRA_BUNNIES_KEY = "#@$#ZXQA";

	private const string EARTHQUAKE_KEY = "#@$#5aaz3";

	private const string PACK_UNLOCKED_KEY = "6222zxcaFFFZXC@@#$!@AD";

	private const string LEVEL_UNLOCKED_KEY = "65zzxcaFFFZXC@@#$!@AD";

	private const string LAST_PACK_PLAYED = "89asczzzzzzzxX";

	private const string LAST_LEVEL_PLAYED = "sf6fx85SSSXX";

	public static int ExtraProjectilesCount
	{
		get
		{
			return SaveManager.LoadInt("#@$#ZXQA");
		}
		set
		{
			SaveManager.SaveInt("#@$#ZXQA", value);
		}
	}

	public static int QuakesCount
	{
		get
		{
			return SaveManager.LoadInt("#@$#5aaz3");
		}
		set
		{
			SaveManager.SaveInt("#@$#5aaz3", value);
		}
	}

	public static int LastPackUnlocked
	{
		get
		{
			return SaveManager.LoadInt("6222zxcaFFFZXC@@#$!@AD");
		}
		set
		{
			SaveManager.SaveInt("6222zxcaFFFZXC@@#$!@AD", value);
		}
	}

	public static int LastPackPlayed
	{
		get
		{
			return SaveManager.LoadInt("89asczzzzzzzxX");
		}
		set
		{
			SaveManager.SaveInt("89asczzzzzzzxX", value);
		}
	}

	public static int GetLastLevelUnlocked(int pack)
	{
		return SaveManager.LoadInt("65zzxcaFFFZXC@@#$!@AD" + pack);
	}

	public static void SetLastLevelUnlocked(int pack, int value)
	{
		SaveManager.SaveInt("65zzxcaFFFZXC@@#$!@AD" + pack, value);
	}

	public static int GetLastLevelPlayed(int packID)
	{
		return SaveManager.LoadInt("sf6fx85SSSXX" + packID);
	}

	public static void SetLastLevelPlayed(int packID, int value)
	{
		SaveManager.SaveInt("sf6fx85SSSXX" + packID, value);
	   // log("packID - "+ packID + ", value "+ value);

	}
}
