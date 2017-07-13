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
	bool m_detectedFirstTouch = false;

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

		if (!m_detectedFirstTouch)
		{
			GameController.Instance.StartRevealOfImReadyButton ();
		}
		m_detectedFirstTouch = true;
	}

	public void StartFreeRotation()
	{
		m_detectedFirstTouch = false;
	}
}
