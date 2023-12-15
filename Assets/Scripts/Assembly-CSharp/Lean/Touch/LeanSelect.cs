using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lean.Touch
{
	public class LeanSelect : MonoBehaviour
	{
		public enum SelectType
		{
			Raycast3D = 0,
			Overlap2D = 1,
			CanvasUI = 2
		}

		public enum SearchType
		{
			GetComponent = 0,
			GetComponentInParent = 1,
			GetComponentInChildren = 2
		}

		public enum ReselectType
		{
			KeepSelected = 0,
			Deselect = 1,
			DeselectAndSelect = 2,
			SelectAgain = 3
		}

		public SelectType SelectUsing;

		[Tooltip("This stores the layers we want the raycast/overlap to hit (make sure this GameObject's layer is included!)")]
		public LayerMask LayerMask = -5;

		[Tooltip("The camera used to calculate the ray (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("How should the selected GameObject be searched for the LeanSelectable component?")]
		public SearchType Search;

		[Tooltip("The currently selected LeanSelectables")]
		public List<LeanSelectable> CurrentSelectables;

		[Tooltip("If you select an already selected selectable, what should happen?")]
		public ReselectType Reselect;

		[Tooltip("Automatically deselect the CurrentSelectable if Select gets called with null?")]
		public bool AutoDeselect;

		[Tooltip("Automatically deselect a LeanSelectable when the selecting finger goes up?")]
		public bool DeselectOnUp;

		[Tooltip("The maximum amount of selectables that can be selected at once (0 = Unlimited)")]
		public int MaxSelectables;

		public void SelectStartScreenPosition(LeanFinger finger)
		{
			SelectScreenPosition(finger, finger.StartScreenPosition);
		}

		public void SelectScreenPosition(LeanFinger finger)
		{
			SelectScreenPosition(finger, finger.ScreenPosition);
		}

		public void SelectScreenPosition(LeanFinger finger, Vector2 screenPosition)
		{
			Component component = null;
			switch (SelectUsing)
			{
			case SelectType.Raycast3D:
			{
				Camera camera2 = LeanTouch.GetCamera(Camera, base.gameObject);
				if (camera2 != null)
				{
					Ray ray = camera2.ScreenPointToRay(screenPosition);
					RaycastHit hitInfo = default(RaycastHit);
					if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, LayerMask))
					{
						component = hitInfo.collider;
					}
				}
				break;
			}
			case SelectType.Overlap2D:
			{
				Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
				if (camera != null)
				{
					Vector3 vector = camera.ScreenToWorldPoint(screenPosition);
					component = Physics2D.OverlapPoint(vector, LayerMask);
				}
				break;
			}
			case SelectType.CanvasUI:
			{
				List<RaycastResult> list = LeanTouch.RaycastGui(screenPosition, LayerMask);
				if (list != null && list.Count > 0)
				{
					component = list[0].gameObject.transform;
				}
				break;
			}
			}
			Select(finger, component);
		}

		public void Select(LeanFinger finger, Component component)
		{
			LeanSelectable selectable = null;
			if (component != null)
			{
				switch (Search)
				{
				case SearchType.GetComponent:
					selectable = component.GetComponent<LeanSelectable>();
					break;
				case SearchType.GetComponentInParent:
					selectable = component.GetComponentInParent<LeanSelectable>();
					break;
				case SearchType.GetComponentInChildren:
					selectable = component.GetComponentInChildren<LeanSelectable>();
					break;
				}
			}
			Select(finger, selectable);
		}

		public LeanSelectable FindSelectable(LeanFinger finger)
		{
			for (int num = CurrentSelectables.Count - 1; num >= 0; num--)
			{
				LeanSelectable leanSelectable = CurrentSelectables[num];
				if (leanSelectable.SelectingFinger == finger)
				{
					return leanSelectable;
				}
			}
			return null;
		}

		public bool IsSelected(LeanSelectable selectable)
		{
			for (int num = CurrentSelectables.Count - 1; num >= 0; num--)
			{
				LeanSelectable leanSelectable = CurrentSelectables[num];
				if (leanSelectable == selectable)
				{
					return true;
				}
			}
			return false;
		}

		public void Select(LeanFinger finger, List<LeanSelectable> selectables)
		{
			int num = 0;
			if (CurrentSelectables != null)
			{
				for (int num2 = CurrentSelectables.Count - 1; num2 >= 0; num2--)
				{
					LeanSelectable leanSelectable = CurrentSelectables[num2];
					if (leanSelectable != null && selectables != null && !selectables.Contains(leanSelectable))
					{
						CurrentSelectables.RemoveAt(num2);
						leanSelectable.Deselect();
					}
				}
			}
			if (selectables != null)
			{
				for (int num3 = selectables.Count - 1; num3 >= 0; num3--)
				{
					LeanSelectable leanSelectable2 = selectables[num3];
					if (leanSelectable2 != null)
					{
						if (CurrentSelectables == null || !CurrentSelectables.Contains(leanSelectable2))
						{
							Select(finger, leanSelectable2);
						}
						num++;
					}
				}
			}
			if (num == 0 && AutoDeselect)
			{
				DeselectAll();
			}
		}

		public void Select(LeanFinger finger, LeanSelectable selectable)
		{
			if (selectable != null && selectable.isActiveAndEnabled)
			{
				if (selectable.HideWithFinger)
				{
					for (int num = CurrentSelectables.Count - 1; num >= 0; num--)
					{
						LeanSelectable leanSelectable = CurrentSelectables[num];
						if (leanSelectable.HideWithFinger && leanSelectable.IsSelected)
						{
							return;
						}
					}
				}
				if (!IsSelected(selectable))
				{
					if (MaxSelectables > 0)
					{
						int num2 = CurrentSelectables.Count - MaxSelectables + 1;
						for (int num3 = num2 - 1; num3 >= 0; num3--)
						{
							LeanSelectable leanSelectable2 = CurrentSelectables[num3];
							leanSelectable2.Deselect();
							CurrentSelectables.RemoveAt(num3);
						}
					}
					CurrentSelectables.Add(selectable);
					selectable.Select(finger);
					return;
				}
				switch (Reselect)
				{
				case ReselectType.Deselect:
					selectable.Deselect();
					CurrentSelectables.Remove(selectable);
					break;
				case ReselectType.DeselectAndSelect:
					selectable.Deselect();
					selectable.Select(finger);
					break;
				case ReselectType.SelectAgain:
					selectable.Select(finger);
					break;
				}
			}
			else if (AutoDeselect)
			{
				DeselectAll();
			}
		}

		[ContextMenu("Deselect All")]
		public void DeselectAll()
		{
			if (CurrentSelectables == null)
			{
				return;
			}
			for (int num = CurrentSelectables.Count - 1; num >= 0; num--)
			{
				LeanSelectable leanSelectable = CurrentSelectables[num];
				if (leanSelectable != null)
				{
					leanSelectable.Deselect();
				}
			}
			CurrentSelectables.Clear();
		}

		protected virtual void Update()
		{
			if (!DeselectOnUp || CurrentSelectables == null)
			{
				return;
			}
			for (int num = CurrentSelectables.Count - 1; num >= 0; num--)
			{
				LeanSelectable leanSelectable = CurrentSelectables[num];
				if (leanSelectable != null && leanSelectable.SelectingFinger != null && !leanSelectable.SelectingFinger.Set)
				{
					leanSelectable.Deselect();
					CurrentSelectables.RemoveAt(num);
				}
			}
		}
	}
}
