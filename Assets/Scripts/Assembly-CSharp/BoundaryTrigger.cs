using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Collider2D componentInChildren = collision.GetComponentInChildren<Collider2D>();
		if (componentInChildren.tag == "Player")
		{
			componentInChildren.GetComponentInChildren<ProjectileController>().Dismiss(3f);
		}
	}
}
