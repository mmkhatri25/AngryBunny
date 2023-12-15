using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanSelectable : MonoBehaviour
	{
		[Serializable]
		public class LeanFingerEvent : UnityEvent<LeanFinger>
		{
		}

		public static List<LeanSelectable> Instances = new List<LeanSelectable>();

		[Tooltip("Should IsSelected temporarily return false if the selecting finger is still being held?")]
		public bool HideWithFinger;

		[NonSerialized]
		public LeanFinger SelectingFinger;

		public LeanFingerEvent OnSelect;

		public LeanFingerEvent OnSelectUp;

		public UnityEvent OnDeselect;

		[SerializeField]
		private bool isSelected;

		public bool IsSelected
		{
			get
			{
				if (HideWithFinger && isSelected && SelectingFinger != null)
				{
					return false;
				}
				return isSelected;
			}
		}

		[ContextMenu("Select")]
		public void Select()
		{
			Select(null);
		}

		public void Select(LeanFinger finger)
		{
			isSelected = true;
			SelectingFinger = finger;
			if (OnSelect != null)
			{
				OnSelect.Invoke(finger);
			}
		}

		[ContextMenu("Deselect")]
		public void Deselect()
		{
			if (isSelected)
			{
				isSelected = false;
				if (SelectingFinger != null && OnSelectUp != null)
				{
					OnSelectUp.Invoke(SelectingFinger);
				}
				if (OnDeselect != null)
				{
					OnDeselect.Invoke();
				}
			}
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
			if (isSelected)
			{
				Deselect();
			}
		}

		protected virtual void LateUpdate()
		{
			if (SelectingFinger != null && (!SelectingFinger.Set || !isSelected))
			{
				SelectingFinger = null;
			}
		}
	}
}
