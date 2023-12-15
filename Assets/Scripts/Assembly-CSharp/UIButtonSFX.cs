using UnityEngine;
using UnityEngine.UI;

public class UIButtonSFX : MonoBehaviour
{
	private void OnEnable()
	{
		GetComponent<Button>().onClick.AddListener(SFX);
	}

	private void SFX()
	{
		AudioController.PlaySFX(0);
	}
}
