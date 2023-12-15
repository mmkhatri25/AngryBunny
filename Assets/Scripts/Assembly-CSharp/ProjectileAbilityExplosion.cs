using UnityEngine;

public class ProjectileAbilityExplosion : ProjectileAbility
{
	public GameObject explosionPrefab;

	public float explosionForce;

	public float explosionRadius;

	protected override bool Apply(int step)
	{
		if (base.Apply(step))
		{
			GameObject gameObject = Object.Instantiate(explosionPrefab);
			gameObject.transform.position = base.transform.position;
			Explosion componentInChildren = gameObject.GetComponentInChildren<Explosion>();
			componentInChildren.force = explosionForce;
			componentInChildren.radius = explosionRadius;
			componentInChildren.Explode();
			if (m_controller.CurrentState < m_applyInState)
			{
				m_controller.CurrentState = ProjectileController.State.Crashed;
				m_controller.CurrentState = ProjectileController.State.OnDestroy;
			}
			return true;
		}
		return false;
	}

	protected override void ControllerStateChanged(ProjectileController.State state)
	{
		base.ControllerStateChanged(state);
	}
}
