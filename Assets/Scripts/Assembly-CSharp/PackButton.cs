using UnityEngine;

public class PackButton : MonoBehaviour
{
	[SerializeField]
	private int m_packID;

	[SerializeField]
	private GameObject[] lockObjects;

	private void SetLocked(bool state)
	{
		for (int i = 0; i < lockObjects.Length; i++)
		{
			lockObjects[i].SetActive(state);
		}
	}

	public void Lock()
	{
		SetLocked(true);
	}

	public void Unlock()
	{
		SetLocked(false);
	}

	private void OnEnable()
	{
		SetLocked(LevelController.IsPackLocked(m_packID));
	}
}
