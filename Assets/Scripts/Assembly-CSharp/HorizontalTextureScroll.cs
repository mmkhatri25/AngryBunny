using UnityEngine;

public class HorizontalTextureScroll : MonoBehaviour
{
	public Transform target;

	public float speed;

	private Vector3 m_startPos;

	private Material m_material;

	public Vector2 Offset
	{
		get
		{
			Vector2 result = target.position - m_startPos;
			result.y = 0f;
			return result;
		}
	}

	private void Awake()
	{
		m_startPos = target.position;
		m_material = GetComponentInChildren<MeshRenderer>().material;
	}

	private void Update()
	{
		m_material.mainTextureOffset = Offset * speed;
	}
}
