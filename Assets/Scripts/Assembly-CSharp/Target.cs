using UnityEngine;

public class Target : MonoBehaviour
{
	[SerializeField]
	private TargetsController.TargetType m_type;

	private void Awake()
	{
		UpdateReference();
	}

	public void UpdateReference()
	{
		Utils.ClearChildren(base.transform);
		GameObject gameObject = Object.Instantiate(TargetsController.GetTargetPrefab(m_type), base.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
		base.name = m_type.ToString();
	}
}
