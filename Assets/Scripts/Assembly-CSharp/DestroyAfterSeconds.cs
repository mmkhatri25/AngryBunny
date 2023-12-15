using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
	public float destructionTime = 3f;

	private void Start()
	{
		Invoke("DestroySelf", destructionTime);
	}

	private void OnDisable()
	{
		CancelInvoke("DestroySelf");
	}

	private void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}
