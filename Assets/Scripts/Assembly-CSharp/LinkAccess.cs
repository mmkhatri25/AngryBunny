using UnityEngine;

public class LinkAccess : MonoBehaviour
{
	public string url = "www.google.com";

	public void Access()
	{
		Application.OpenURL(url);
	}
}
