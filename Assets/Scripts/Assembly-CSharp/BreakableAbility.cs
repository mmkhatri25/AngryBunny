using System;
using UnityEngine;

public class BreakableAbility : MonoBehaviour
{
	private void Start()
	{
		Breakable component = GetComponent<Breakable>();
		component.OnDestroyed = (Breakable.BreakableObjectEvent)Delegate.Combine(component.OnDestroyed, new Breakable.BreakableObjectEvent(Apply));
	}

	public virtual void Apply(GameObject breakable)
	{
	}
}
