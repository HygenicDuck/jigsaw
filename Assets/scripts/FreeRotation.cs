using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class FreeRotation : MessagingManager {

//	const float MAX_SWIPE_TIME = 1.0f;
//	const float MIN_SWIPE_DISTANCE = 1.0f;

	Vector2 m_touchPosition;
	Vector2 m_touchDownPosition;
	Vector2 m_deltaTouchPos;
	bool m_touching;
	float m_timeDown;
	Vector2 m_spinningSpeed;

	[SerializeField] 
	GameObject m_controlledObject;


	public enum SwipeDirection
	{
		up,
		down,
		left,
		right
	}


	// Use this for initialization
	void Start () 
	{
		m_spinningSpeed = new Vector2(0.1f, 0.3f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			m_touching = true;
			m_touchPosition = Input.mousePosition;
			m_touchDownPosition = Input.mousePosition;
			m_deltaTouchPos = Vector2.zero;

			m_timeDown = Time.timeSinceLevelLoad;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			m_touching = false;
			//DetectSwipe();
		}
		else if (Input.GetMouseButton(0))
		{
			Vector2 pos = Input.mousePosition;
			m_deltaTouchPos = pos - m_touchPosition;
			m_touchPosition = pos;
			DoRotation();
			m_spinningSpeed = Vector2.zero;
		}
		else if (m_touching)
		{
			m_touching = false;
		}
		else
		{
			// spinning freely
			Quaternion currentRot = m_controlledObject.transform.localRotation;
			Quaternion dXRot = Quaternion.AngleAxis(m_spinningSpeed.x, Vector3.up);
			Quaternion dYRot = Quaternion.AngleAxis(m_spinningSpeed.y, Vector3.right);
			m_controlledObject.transform.localRotation = dXRot * dYRot * currentRot;
		}
	}

	void DoRotation()
	{
		const float ROTATION_SPEED = 0.5f;
		Quaternion currentRot = m_controlledObject.transform.localRotation;
		Quaternion dXRot = Quaternion.AngleAxis(-m_deltaTouchPos.x * ROTATION_SPEED, Vector3.up);
		Quaternion dYRot = Quaternion.AngleAxis(m_deltaTouchPos.y * ROTATION_SPEED, Vector3.right);
		m_controlledObject.transform.localRotation = dXRot * dYRot * currentRot;
	}

//	void DetectSwipe()
//	{
//		float touchTime = Time.timeSinceLevelLoad - m_timeDown;
//		if (touchTime < MAX_SWIPE_TIME)
//		{
//			Vector2 swipeVec = m_touchPosition - m_touchDownPosition;
//			float distance = swipeVec.magnitude;
//			if (distance > MIN_SWIPE_DISTANCE)
//			{
//				if ((swipeVec.y < 0f) && (Mathf.Abs(swipeVec.y) > Mathf.Abs(swipeVec.x)))
//				{
//					// swiped down
//					Debug.Log("SWIPE DOWN");
//					TriggerEvent("SWIPE_DOWN");
//				}
//				if ((swipeVec.y > 0f) && (Mathf.Abs(swipeVec.y) > Mathf.Abs(swipeVec.x)))
//				{
//					// swiped up
//					Debug.Log("SWIPE UP");
//					TriggerEvent("SWIPE_UP");
//				}
//				if ((swipeVec.x < 0f) && (Mathf.Abs(swipeVec.y) < Mathf.Abs(swipeVec.x)))
//				{
//					// swiped left
//					Debug.Log("SWIPE LEFT");
//					TriggerEvent("SWIPE_LEFT");
//				}
//				if ((swipeVec.x > 0f) && (Mathf.Abs(swipeVec.y) < Mathf.Abs(swipeVec.x)))
//				{
//					// swiped right
//					Debug.Log("SWIPE RIGHT");
//					TriggerEvent("SWIPE_RIGHT");
//				}
//
//			}
//		}
//	}

//	public void TouchDown(BaseEventData data)
//	{
//		PointerEventData pointerData = data as PointerEventData;
//		m_touchPosition = pointerData.position;
//		Debug.Log ("TouchDown = "+m_touchPosition);
//
////		//touchPressure = 0f;
////		TouchDraw(touchPosition);
////		//drawingSound.volume = 0.5f;
////		PositionSparksEmitter(touchPosition);
//	}
//
//	public void TouchUp(BaseEventData data)
//	{
//		Debug.Log ("TouchUp = ");
//		//drawingSound.volume = 0f;
//		//EnableSparksEmitters(false);
//	}
//
//	public void TouchMove(BaseEventData data)
//	{
//		PointerEventData pointerData = data as PointerEventData;
//		m_touchPosition = pointerData.position;
//		Debug.Log ("TouchMove = "+m_touchPosition);
////		TouchDraw(touchPosition);
////		PositionSparksEmitter(touchPosition);
//	}
}
