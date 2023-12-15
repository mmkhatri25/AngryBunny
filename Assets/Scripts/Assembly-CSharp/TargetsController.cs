using UnityEngine;

public class TargetsController : MonoBehaviour
{
	public enum TargetType
	{
		Cat = 0,
		Wood_Long = 1,
		Wood_Square = 2,
		Rock_Long = 3,
		Rock_Square = 4,
		Glass_Long = 5,
		Glass_Square = 6,
		TNT = 7
	}

	[SerializeField]
	private GameObject[] m_targetsPrefabs;

	private static TargetsController m_instance;

	private static TargetsController Instance
	{
		get
		{
			if (!m_instance)
			{
				m_instance = Object.FindObjectOfType<TargetsController>();
			}
			return m_instance;
		}
	}

	public static GameObject GetTargetPrefab(TargetType type)
	{
		return Instance.m_targetsPrefabs[(int)type];
	}
}
