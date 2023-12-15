using System;
using System.IO;
using UnityEngine;

public class SaveManager
{
	public const int VALUE_NULL = -1;

	internal static readonly string dayKey = "DJAG";

	internal static readonly string monthKey = "FAS@$$CZ6524466a21";

	internal static readonly string yearKey = "asczx24684a@$";

	internal static readonly string hourKey = "HASDF*&@$K24178BIF&";

	internal static readonly string minuteKey = "OLIJX(C*C*ZXCSA";

	internal static readonly string secondKey = "ZUS&*S&SA*(D&";

	private const float _encodeValue = -18.52f;

	public static void ResetAll()
	{
		PlayerPrefs.DeleteAll();
	}

	private static int encrypt(int value)
	{
		return value + -18;
	}

	private static int decrypt(int value)
	{
		return value - -18;
	}

	private static float encrypt(float value)
	{
		return value + -18.52f;
	}

	private static float decrypt(float value)
	{
		return value - -18.52f;
	}

	public static void SaveInt(string key, int value)
	{
		PlayerPrefs.SetInt(key, encrypt(value));
	}

	public static int LoadInt(string key)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			SaveInt(key, 0);
		}
		return decrypt(PlayerPrefs.GetInt(key));
	}

	public static void ModifyInt(string key, int value)
	{
		SaveInt(key, LoadInt(key) + value);
	}

	public static void SaveFloat(string key, float value)
	{
		PlayerPrefs.SetFloat(key, encrypt(value));
	}

	public static float LoadFloat(string key)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			SaveFloat(key, 0f);
		}
		return decrypt(PlayerPrefs.GetFloat(key));
	}

	public static void ModifyFloat(string key, float value)
	{
		SaveFloat(key, LoadFloat(key) + value);
	}

	public static void Remove(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}

	public static void SaveString(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
	}

	public static string LoadString(string key)
	{
		return PlayerPrefs.GetString(key);
	}

	public static void SaveDate(string key, DateTime dateTime)
	{
		SaveInt(yearKey + key, dateTime.Year);
		SaveInt(monthKey + key, dateTime.Month);
		SaveInt(dayKey + key, dateTime.Day);
		SaveInt(hourKey + key, dateTime.Hour);
		SaveInt(minuteKey + key, dateTime.Minute);
		SaveInt(secondKey + key, dateTime.Second);
	}

	public static DateTime LoadDate(string key)
	{
		if (LoadInt(yearKey + key) == 0)
		{
			return DateTime.MinValue;
		}
		return new DateTime(LoadInt(yearKey + key), LoadInt(monthKey + key), LoadInt(dayKey + key), LoadInt(hourKey + key), LoadInt(minuteKey + key), LoadInt(secondKey + key));
	}

	public static Texture2D SpriteToTexture2D(Sprite sprite)
	{
		Texture2D texture2D = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
		Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
		texture2D.SetPixels(pixels);
		texture2D.Apply();
		return texture2D;
	}

	public static bool SaveSpriteToFile(Sprite sprite, string fileName)
	{
		return SaveTextureToFile(SpriteToTexture2D(sprite), fileName);
	}

	public static bool SaveTextureToFile(Texture2D texture, string fileName)
	{
		string text = Application.persistentDataPath + "/" + fileName;
		string path = text.Remove(text.LastIndexOf('/'));
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		byte[] buffer = texture.EncodeToPNG();
		FileStream fileStream = File.Create(text);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(buffer);
		fileStream.Close();
		return true;
	}

	public static Sprite LoadSprite(string fileName, int width, int height)
	{
		Texture2D texture2D = LoadTexture2D(fileName, width, height);
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
	}

	public static void RemoveFile(string fileName)
	{
		string path = Application.persistentDataPath + "/" + fileName;
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	public static Texture2D LoadTexture2D(string fileName, int width, int height)
	{
		string text = Application.persistentDataPath + "/" + fileName;
		FileStream fileStream = File.Open(text, FileMode.Open);
		if (fileStream == null)
		{
			Debug.LogError("Couldn't open file " + text);
			return null;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.LoadImage(ReadFully(fileStream));
		fileStream.Close();
		return texture2D;
	}

	public static bool FileExists(string fileName)
	{
		return File.Exists(Application.persistentDataPath + "/" + fileName);
	}

	public static byte[] ReadFully(Stream input)
	{
		byte[] array = new byte[2048];
		using (MemoryStream memoryStream = new MemoryStream())
		{
			int count;
			while ((count = input.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, count);
			}
			return memoryStream.ToArray();
		}
	}
}
