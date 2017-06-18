using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class TouchSwipeDetector : MessagingManager {

	const float MAX_SWIPE_TIME = 1.0f;
	const float MIN_SWIPE_DISTANCE = 1.0f;
	const float ROTATE_SWIPE_AREA_HEIGHT = 0.2f;

	Vector2 m_touchPosition;
	Vector2 m_touchDownPosition;
	bool m_touching;
	float m_timeDown;
	bool m_zRotationSwipe = false;

	public enum SwipeDirection
	{
		up,
		down,
		left,
		right
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			m_touching = true;
			m_touchPosition = Input.mousePosition;
			m_touchDownPosition = Input.mousePosition;

			m_zRotationSwipe = ((m_touchDownPosition.y < Screen.height * ROTATE_SWIPE_AREA_HEIGHT) || (m_touchDownPosition.y > Screen.height * (1f - ROTATE_SWIPE_AREA_HEIGHT)));

			//Debug.Log("screen size = x="+Screen.width+", y="+Screen.height);
			//Debug.Log("m_touchDownPosition = x="+m_touchDownPosition.x+", y="+m_touchDownPosition.y);

			m_timeDown = Time.timeSinceLevelLoad;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			m_touching = false;
			DetectSwipe();
		}
		else if (Input.GetMouseButton(0))
		{
			m_touchPosition = Input.mousePosition;
		}
		else if (m_touching)
		{
			m_touching = false;
		}
	}

	void DetectSwipe()
	{
		float touchTime = Time.timeSinceLevelLoad - m_timeDown;
		if (touchTime < MAX_SWIPE_TIME)
		{
			Vector2 swipeVec = m_touchPosition - m_touchDownPosition;
			float distance = swipeVec.magnitude;
			if (distance > MIN_SWIPE_DISTANCE)
			{
				if ((swipeVec.y < 0f) && (Mathf.Abs(swipeVec.y) > Mathf.Abs(swipeVec.x)))
				{
					// swiped down
					//Debug.Log("SWIPE DOWN");
					TriggerEvent("SWIPE_DOWN");
				}
				if ((swipeVec.y > 0f) && (Mathf.Abs(swipeVec.y) > Mathf.Abs(swipeVec.x)))
				{
					// swiped up
					//Debug.Log("SWIPE UP");
					TriggerEvent("SWIPE_UP");
				}
				if ((swipeVec.x < 0f) && (Mathf.Abs(swipeVec.y) < Mathf.Abs(swipeVec.x)))
				{
					// swiped left
					//Debug.Log("SWIPE LEFT");
					if (m_zRotationSwipe)
					{
						if (m_touchDownPosition.y > Screen.height * 0.5f)
						{
							//Debug.Log("SWIPE_ANTICLOCKWISE");
							TriggerEvent("SWIPE_ANTICLOCKWISE");
						}
						else
						{
							//Debug.Log("SWIPE_CLOCKWISE");
							TriggerEvent("SWIPE_CLOCKWISE");
						}
					}
					else
					{
						TriggerEvent("SWIPE_LEFT");
					}
				}
				if ((swipeVec.x > 0f) && (Mathf.Abs(swipeVec.y) < Mathf.Abs(swipeVec.x)))
				{
					// swiped right
					//Debug.Log("SWIPE RIGHT");
					if (m_zRotationSwipe)
					{
						if (m_touchDownPosition.y > Screen.height * 0.5f)
						{
							TriggerEvent("SWIPE_CLOCKWISE");
						}
						else
						{
							TriggerEvent("SWIPE_ANTICLOCKWISE");
						}
					}
					else
					{
						TriggerEvent("SWIPE_RIGHT");
					}
				}

			}
		}
	}

}
