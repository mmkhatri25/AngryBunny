using System;
using UnityEngine;

namespace Lean.Touch
{
	public abstract class LeanSelectableBehaviour : MonoBehaviour
	{
		[NonSerialized]
		private LeanSelectable selectable;

		public LeanSelectable Selectable
		{
			get
			{
				if (selectable == null)
				{
					UpdateSelectable();
				}
				return selectable;
			}
		}

		protected virtual void OnEnable()
		{
			UpdateSelectable();
			selectable.OnSelect.AddListener(OnSelect);
			selectable.OnSelectUp.AddListener(OnSelectUp);
			selectable.OnDeselect.AddListener(OnDeselect);
		}

		protected virtual void OnDisable()
		{
			UpdateSelectable();
			selectable.OnSelect.RemoveListener(OnSelect);
			selectable.OnSelectUp.RemoveListener(OnSelectUp);
			selectable.OnDeselect.RemoveListener(OnDeselect);
		}

		protected virtual void OnSelect(LeanFinger finger)
		{
		}

		protected virtual void OnSelectUp(LeanFinger finger)
		{
		}

		protected virtual void OnDeselect()
		{
		}

		private void UpdateSelectable()
		{
			if (selectable == null)
			{
				selectable = GetComponentInParent<LeanSelectable>();
				if (selectable == null)
				{
					Debug.LogError("This GameObject or one of its parents must have the LeanSelectable component.", this);
				}
			}
		}
	}
}
