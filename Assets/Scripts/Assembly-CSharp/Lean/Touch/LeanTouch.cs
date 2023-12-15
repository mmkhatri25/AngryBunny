using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lean.Touch
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class LeanTouch : MonoBehaviour
	{
		public static List<LeanTouch> Instances = new List<LeanTouch>();

		public static List<LeanFinger> Fingers = new List<LeanFinger>(10);

		public static List<LeanFinger> InactiveFingers = new List<LeanFinger>(10);

		public static Action<LeanFinger> OnFingerDown;

		public static Action<LeanFinger> OnFingerSet;

		public static Action<LeanFinger> OnFingerUp;

		public static Action<LeanFinger> OnFingerTap;

		public static Action<LeanFinger> OnFingerSwipe;

		public static Action<List<LeanFinger>> OnGesture;

		[Tooltip("This allows you to set how many seconds are required between a finger down/up for a tap to be registered")]
		public float TapThreshold = 0.5f;

		public const float DefaultTapThreshold = 0.5f;

		[Tooltip("This allows you to set how many pixels of movement (relative to the ReferenceDpi) are required within the TapThreshold for a swipe to be triggered")]
		public float SwipeThreshold = 50f;

		public const float DefaultSwipeThreshold = 50f;

		[Tooltip("This allows you to set the default DPI you want the input scaling to be based on")]
		public int ReferenceDpi = 200;

		public const int DefaultReferenceDpi = 200;

		[Tooltip("This allows you to set which layers your GUI is on, so it can be ignored by each finger")]
		public LayerMask GuiLayers = -5;

		[Tooltip("This allows you to enable recording of finger movements")]
		public bool RecordFingers = true;

		[Tooltip("This allows you to set the amount of pixels a finger must move for another snapshot to be stored")]
		public float RecordThreshold = 5f;

		[Tooltip("This allows you to set the maximum amount of seconds that can be recorded, 0 = unlimited")]
		public float RecordLimit = 10f;

		[Tooltip("This allows you to simulate multi touch inputs on devices that don't support them (e.g. desktop)")]
		public bool SimulateMultiFingers = true;

		[Tooltip("This allows you to set which key is required to simulate multi key twisting")]
		public KeyCode PinchTwistKey = KeyCode.LeftControl;

		[Tooltip("This allows you to set which key is required to simulate multi key dragging")]
		public KeyCode MultiDragKey = KeyCode.LeftAlt;

		[Tooltip("This allows you to set which texture will be used to show the simulated fingers")]
		public Texture2D FingerTexture;

		private static int highestMouseButton = 7;

		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		private static List<LeanFinger> filteredFingers = new List<LeanFinger>(10);

		private static PointerEventData tempPointerEventData;

		private static EventSystem tempEventSystem;

		public static float CurrentTapThreshold
		{
			get
			{
				return (Instances.Count <= 0) ? 0.5f : Instances[0].TapThreshold;
			}
		}

		public static float CurrentSwipeThreshold
		{
			get
			{
				return (Instances.Count <= 0) ? 50f : Instances[0].SwipeThreshold;
			}
		}

		public static int CurrentReferenceDpi
		{
			get
			{
				return (Instances.Count <= 0) ? 200 : Instances[0].ReferenceDpi;
			}
		}

		public static LayerMask CurrentGuiLayers
		{
			get
			{
				return (Instances.Count <= 0) ? ((LayerMask)(-5)) : Instances[0].GuiLayers;
			}
		}

		public static LeanTouch Instance
		{
			get
			{
				return (Instances.Count <= 0) ? null : Instances[0];
			}
		}

		public static float ScalingFactor
		{
			get
			{
				float dpi = Screen.dpi;
				if (dpi <= 0f)
				{
					return 1f;
				}
				return Mathf.Sqrt(CurrentReferenceDpi) / Mathf.Sqrt(dpi);
			}
		}

		public static bool AnyMouseButtonSet
		{
			get
			{
				for (int i = 0; i < highestMouseButton; i++)
				{
					if (Input.GetMouseButton(i))
					{
						return true;
					}
				}
				return false;
			}
		}

		public static bool GuiInUse
		{
			get
			{
				if (GUIUtility.hotControl > 0)
				{
					return true;
				}
				for (int num = Fingers.Count - 1; num >= 0; num--)
				{
					if (Fingers[num].StartedOverGui)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static Camera GetCamera(Camera currentCamera, GameObject gameObject = null)
		{
			if (currentCamera == null)
			{
				if (gameObject != null)
				{
					currentCamera = gameObject.GetComponent<Camera>();
				}
				if (currentCamera == null)
				{
					currentCamera = Camera.main;
				}
			}
			return currentCamera;
		}

		public static float GetDampenFactor(float dampening, float deltaTime)
		{
			if (!Application.isPlaying)
			{
				return 1f;
			}
			return 1f - Mathf.Exp((0f - dampening) * deltaTime);
		}

		public static bool PointOverGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition).Count > 0;
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition, CurrentGuiLayers);
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
		{
			tempRaycastResults.Clear();
			EventSystem current = EventSystem.current;
			if (current != null)
			{
				if (current != tempEventSystem)
				{
					tempEventSystem = current;
					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}
				tempPointerEventData.position = screenPosition;
				current.RaycastAll(tempPointerEventData, tempRaycastResults);
				if (tempRaycastResults.Count > 0)
				{
					for (int num = tempRaycastResults.Count - 1; num >= 0; num--)
					{
						int num2 = 1 << tempRaycastResults[num].gameObject.layer;
						if ((num2 & (int)layerMask) == 0)
						{
							tempRaycastResults.RemoveAt(num);
						}
					}
				}
			}
			return tempRaycastResults;
		}

		public static List<LeanFinger> GetFingers(bool ignoreGuiFingers, int requiredFingerCount = 0, LeanSelectable requiredSelectable = null)
		{
			filteredFingers.Clear();
			if (requiredSelectable != null && requiredSelectable.SelectingFinger != null)
			{
				filteredFingers.Add(requiredSelectable.SelectingFinger);
				return filteredFingers;
			}
			for (int i = 0; i < Fingers.Count; i++)
			{
				LeanFinger leanFinger = Fingers[i];
				if (ignoreGuiFingers)
				{
					if (!leanFinger.StartedOverGui)
					{
						filteredFingers.Add(leanFinger);
					}
				}
				else
				{
					filteredFingers.Add(leanFinger);
				}
			}
			if (requiredFingerCount > 0 && filteredFingers.Count != requiredFingerCount)
			{
				return null;
			}
			return filteredFingers;
		}

		protected virtual void Awake()
		{
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
		}

		protected virtual void Update()
		{
			if (Instances[0] == this)
			{
				BeginFingers();
				PollFingers();
				EndFingers();
				UpdateEvents();
			}
		}

		protected virtual void OnGUI()
		{
			if (!(FingerTexture != null) || Input.touchCount != 0 || Fingers.Count <= 1)
			{
				return;
			}
			for (int num = Fingers.Count - 1; num >= 0; num--)
			{
				LeanFinger leanFinger = Fingers[num];
				if (!leanFinger.Up)
				{
					Vector2 screenPosition = leanFinger.ScreenPosition;
					Rect position = new Rect(0f, 0f, FingerTexture.width, FingerTexture.height);
					position.center = new Vector2(screenPosition.x, (float)Screen.height - screenPosition.y);
					GUI.DrawTexture(position, FingerTexture);
				}
			}
		}

		private void BeginFingers()
		{
			for (int num = InactiveFingers.Count - 1; num >= 0; num--)
			{
				InactiveFingers[num].Age += Time.unscaledDeltaTime;
			}
			for (int num2 = Fingers.Count - 1; num2 >= 0; num2--)
			{
				LeanFinger leanFinger = Fingers[num2];
				if (leanFinger.Up)
				{
					Fingers.RemoveAt(num2);
					InactiveFingers.Add(leanFinger);
					leanFinger.Age = 0f;
					leanFinger.ClearSnapshots();
				}
				else
				{
					leanFinger.LastSet = leanFinger.Set;
					leanFinger.LastScreenPosition = leanFinger.ScreenPosition;
					leanFinger.Set = false;
					leanFinger.Tap = false;
					leanFinger.Swipe = false;
				}
			}
		}

		private void EndFingers()
		{
			for (int num = Fingers.Count - 1; num >= 0; num--)
			{
				LeanFinger leanFinger = Fingers[num];
				if (leanFinger.Up)
				{
					if (leanFinger.Age <= TapThreshold)
					{
						if (leanFinger.SwipeScreenDelta.magnitude * ScalingFactor < SwipeThreshold)
						{
							leanFinger.Tap = true;
							leanFinger.TapCount++;
						}
						else
						{
							leanFinger.TapCount = 0;
							leanFinger.Swipe = true;
						}
					}
					else
					{
						leanFinger.TapCount = 0;
					}
				}
				else if (!leanFinger.Down)
				{
					leanFinger.Age += Time.unscaledDeltaTime;
				}
			}
		}

		private void PollFingers()
		{
			if (Input.touchCount > 0)
			{
				for (int i = 0; i < Input.touchCount; i++)
				{
					UnityEngine.Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
					{
						AddFinger(touch.fingerId, touch.position);
					}
				}
			}
			else
			{
				if (!AnyMouseButtonSet)
				{
					return;
				}
				Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
				Vector2 vector = Input.mousePosition;
				if (!rect.Contains(vector))
				{
					return;
				}
				AddFinger(0, vector);
				if (SimulateMultiFingers)
				{
					if (Input.GetKey(PinchTwistKey))
					{
						Vector2 vector2 = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
						AddFinger(1, vector2 - (vector - vector2));
					}
					else if (Input.GetKey(MultiDragKey))
					{
						AddFinger(1, vector);
					}
				}
			}
		}

		private void UpdateEvents()
		{
			int count = Fingers.Count;
			if (count <= 0)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				LeanFinger leanFinger = Fingers[i];
				if (leanFinger.Down && OnFingerDown != null)
				{
					OnFingerDown(leanFinger);
				}
				if (leanFinger.Set && OnFingerSet != null)
				{
					OnFingerSet(leanFinger);
				}
				if (leanFinger.Up && OnFingerUp != null)
				{
					OnFingerUp(leanFinger);
				}
				if (leanFinger.Tap && OnFingerTap != null)
				{
					OnFingerTap(leanFinger);
				}
				if (leanFinger.Swipe && OnFingerSwipe != null)
				{
					OnFingerSwipe(leanFinger);
				}
			}
			if (OnGesture != null)
			{
				filteredFingers.Clear();
				filteredFingers.AddRange(Fingers);
				OnGesture(filteredFingers);
			}
		}

		private void AddFinger(int index, Vector2 screenPosition)
		{
			LeanFinger leanFinger = FindFinger(index);
			if (leanFinger == null)
			{
				int num = FindInactiveFingerIndex(index);
				if (num >= 0)
				{
					leanFinger = InactiveFingers[num];
					InactiveFingers.RemoveAt(num);
					if (leanFinger.Age > TapThreshold)
					{
						leanFinger.TapCount = 0;
					}
					leanFinger.Age = 0f;
					leanFinger.Set = false;
					leanFinger.LastSet = false;
					leanFinger.Tap = false;
					leanFinger.Swipe = false;
				}
				else
				{
					leanFinger = new LeanFinger();
					leanFinger.Index = index;
				}
				leanFinger.StartScreenPosition = screenPosition;
				leanFinger.LastScreenPosition = screenPosition;
				leanFinger.ScreenPosition = screenPosition;
				leanFinger.StartedOverGui = leanFinger.IsOverGui;
				Fingers.Add(leanFinger);
			}
			leanFinger.Set = true;
			leanFinger.ScreenPosition = screenPosition;
			if (!RecordFingers)
			{
				return;
			}
			if (RecordLimit > 0f && leanFinger.SnapshotDuration > RecordLimit)
			{
				int lowerIndex = LeanSnapshot.GetLowerIndex(leanFinger.Snapshots, leanFinger.Age - RecordLimit);
				leanFinger.ClearSnapshots(lowerIndex);
			}
			if (RecordThreshold > 0f)
			{
				if (leanFinger.Snapshots.Count == 0 || leanFinger.LastSnapshotScreenDelta.magnitude >= RecordThreshold)
				{
					leanFinger.RecordSnapshot();
				}
			}
			else
			{
				leanFinger.RecordSnapshot();
			}
		}

		private LeanFinger FindFinger(int index)
		{
			for (int num = Fingers.Count - 1; num >= 0; num--)
			{
				LeanFinger leanFinger = Fingers[num];
				if (leanFinger.Index == index)
				{
					return leanFinger;
				}
			}
			return null;
		}

		private int FindInactiveFingerIndex(int index)
		{
			for (int num = InactiveFingers.Count - 1; num >= 0; num--)
			{
				if (InactiveFingers[num].Index == index)
				{
					return num;
				}
			}
			return -1;
		}
	}
}
