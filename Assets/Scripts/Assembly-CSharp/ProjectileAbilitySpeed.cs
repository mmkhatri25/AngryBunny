using UnityEngine;

public class ProjectileAbilitySpeed : ProjectileAbility
{
	[SerializeField]
	private float m_multiplier = 2f;

	protected override void Setup()
	{
		base.Setup();
	}

	protected override bool Apply(int step)
	{
		if (base.Apply(step))
		{
			Vector3 vector = GetComponentInChildren<Rigidbody2D>().velocity;
			GetComponentInChildren<Rigidbody2D>().velocity *= m_multiplier * (float)step;
			return true;
		}
		return false;
	}
}
