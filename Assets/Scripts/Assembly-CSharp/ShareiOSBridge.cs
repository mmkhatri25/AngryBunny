using System.Runtime.InteropServices;

public class ShareiOSBridge
{
	[DllImport("__Internal")]
	private static extern void _TAG_ShareTextWithImage(string iosPath, string message);

	[DllImport("__Internal")]
	private static extern void _TAG_ShareSimpleText(string message);

	public static void ShareSimpleText(string message)
	{
	}

	public static void ShareTextWithImage(string imagePath, string message)
	{
	}
}
