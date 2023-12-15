using UnityEngine;

public class FollowTarget : MonoBehaviour
{
	[SerializeField]
	public Transform Target;

	[SerializeField]
	private bool m_Smooth;

	[SerializeField]
	private Vector3 m_Offsets;

	[SerializeField]
	private Vector3 m_Speed;

	private void LateUpdate()
	{
		if ((bool)Target)
		{
			if (m_Smooth)
			{
				Vector3 zero = Vector3.zero;
				zero.x = Mathf.Lerp(base.transform.position.x, Target.position.x + m_Offsets.x, Time.smoothDeltaTime * m_Speed.x);
				zero.y = Mathf.Lerp(base.transform.position.y, Target.position.y + m_Offsets.y, Time.smoothDeltaTime * m_Speed.y);
				zero.z = Mathf.Lerp(base.transform.position.z, Target.position.z + m_Offsets.z, Time.smoothDeltaTime * m_Speed.z);
				base.transform.position = zero;
			}
			else
			{
				Vector3 position = Target.position + m_Offsets;
				position.x = ((m_Speed.x != 0f) ? position.x : base.transform.position.x);
				position.y = ((m_Speed.y != 0f) ? position.y : base.transform.position.y);
				position.z = ((m_Speed.z != 0f) ? position.z : base.transform.position.z);
				base.transform.position = position;
			}
		}
	}
}
