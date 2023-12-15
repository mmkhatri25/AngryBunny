using UnityEngine;

public class BreakableAbilityExplosion : BreakableAbility
{
	public GameObject explosion;

	public float force;

	public float radius;

	public override void Apply(GameObject breakable)
	{
		GetComponentInChildren<Collider2D>().enabled = false;
		GameObject gameObject = Object.Instantiate(explosion);
		gameObject.transform.position = base.transform.position;
		Explosion component = gameObject.GetComponent<Explosion>();
		component.radius = radius;
		component.force = force;
		component.Explode();
	}
}
