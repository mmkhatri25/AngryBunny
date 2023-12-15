using UnityEngine;

public class TouchInput : MonoBehaviour
{
	public delegate void TouchEvent();

	public static TouchEvent OnTouchDown;

	public static TouchEvent OnTouchUp;

	public static bool TouchDown()
	{

#if UNITY_EDITOR
		if (Input.GetMouseButton(0) )//&& !Utils.IsPointerOverUIObject())
		{
            return true;
        }

#else
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !Utils.IsPointerOverUIObject())
		{
			return true;
		}
#endif

            return false;
	}

	public static bool TouchUp()
	{
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) )//&& !Utils.IsPointerOverUIObject())
        {
            return true;
        }

#else
		if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled))
		{
			return true;
		}
#endif

        return false;
	}

	private void Update()
	{
		if (TouchDown() && OnTouchDown != null)
		{
			OnTouchDown();
		}
		if (TouchUp() && OnTouchUp != null)
		{
			OnTouchUp();
		}
	}
}
