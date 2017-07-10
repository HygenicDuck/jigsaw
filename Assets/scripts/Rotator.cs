using UnityEngine;
using System.Collections;

public class Rotator : MessagingManager {

	enum Axis
	{
		X,
		Y,
		Z
	}

	Axis m_rotationAxis;
	Quaternion m_fromRot;
	Quaternion m_toRot;
	bool m_isRotating;
	float m_timer;
	float m_moveDuration;
	bool m_linearInterpolation;
	private AudioSource m_audioSource;
	bool m_rotationBlocked = false;
	CubeState.RotationActions m_lastRotation = CubeState.RotationActions.ANTI_CLOCKWISE;

	[SerializeField] 
	CubeState m_cubeState;
	[SerializeField] 
	AudioClip m_sfxCubeRotate;

	static Rotator m_instance = null;
	static int count = 0;

	public static Rotator Instance
	{
		get
		{
			return m_instance;
		}
	}

	public Rotator()
	{
		Debug.Log ("Rotator constructor "+(count++));
		m_instance = this;
	}


	// Use this for initialization
	void Start () {
		m_isRotating = false;
		m_rotationBlocked = false;

		Debug.Log ("Rotator start");
		m_lastRotation = CubeState.RotationActions.NONE;

		TouchSwipeDetector.Instance.StartListening("SWIPE_LEFT", RotateLeft);
		TouchSwipeDetector.Instance.StartListening("SWIPE_RIGHT", RotateRight);
		TouchSwipeDetector.Instance.StartListening("SWIPE_UP", RotateUp);
		TouchSwipeDetector.Instance.StartListening("SWIPE_DOWN", RotateDown);
		TouchSwipeDetector.Instance.StartListening("SWIPE_CLOCKWISE", RotateClockwise);
		TouchSwipeDetector.Instance.StartListening("SWIPE_ANTICLOCKWISE", RotateAntiClockwise);

		ResetRotation();

		m_audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_isRotating)
		{
			m_timer += Time.deltaTime;
			if (m_timer >= m_moveDuration)
			{
				m_timer = m_moveDuration;
				m_isRotating = false;
				TriggerEvent("ROTATION_COMPLETE");
			}

			float p = m_timer / m_moveDuration;
			if (!m_linearInterpolation)
			{
				p = p*p * (3f - 2f*p);
			}
			transform.rotation = Quaternion.Slerp(m_fromRot, m_toRot, p);
		}
	}

	public void ResetRotation()
	{
		transform.rotation = Quaternion.identity;
	}

	public void RotateTo(Quaternion destination, float time = 1f, bool linear = false)
	{
		m_fromRot = transform.rotation;
		m_toRot = destination;
		m_timer = 0f;
		m_moveDuration = time;
		m_isRotating = true;
		m_linearInterpolation = linear;
	}

	void Rotate90Degrees(int direction, Axis axis, float time)
	{
		float angle = 90f * direction;
		Quaternion rot = Quaternion.identity;
		switch(axis)
		{
		case Axis.X:
			rot = Quaternion.Euler(angle, 0f, 0f);
			break;
		case Axis.Y:
			rot = Quaternion.Euler(0f, angle, 0f);
			break;
		case Axis.Z:
			rot = Quaternion.Euler(0f, 0f, angle);
			break;
		}

		m_fromRot = transform.rotation;
		m_toRot = rot * m_fromRot;	// * rot;
		m_timer = 0f;
		m_moveDuration = time;
		m_isRotating = true;

		if (GameController.Instance.State == GameController.GameState.FIND) 
		{
			m_audioSource.PlayOneShot (m_sfxCubeRotate, 1f);
		}
	}

	public void ReversePreviousRotation()
	{
		Debug.Log ("ReversePreviousRotation "+m_lastRotation);
		switch(m_lastRotation)
		{
		case CubeState.RotationActions.LEFT:
			RotateRight();
			break;
		case CubeState.RotationActions.RIGHT:
			RotateLeft();
			break;
		case CubeState.RotationActions.UP:
			RotateDown();
			break;
		case CubeState.RotationActions.DOWN:
			RotateUp();
			break;
		case CubeState.RotationActions.CLOCKWISE:
			RotateAntiClockwise();
			break;
		case CubeState.RotationActions.ANTI_CLOCKWISE:
			RotateClockwise();
			break;
		}

		m_lastRotation = CubeState.RotationActions.NONE;
	}

	public void RotateLeft()
	{
		if (!m_isRotating && !m_rotationBlocked)
		{
			Rotate90Degrees(1, Axis.Y, 0.3f);
			m_cubeState.DoRotation(CubeState.RotationActions.LEFT);
			m_lastRotation = CubeState.RotationActions.LEFT;
			Debug.Log ("RotateLeft");
		}
	}

	public void RotateRight()
	{
		if (!m_isRotating && !m_rotationBlocked)
		{
			Rotate90Degrees(-1, Axis.Y, 0.3f);
			m_cubeState.DoRotation(CubeState.RotationActions.RIGHT);
			m_lastRotation = CubeState.RotationActions.RIGHT;
			Debug.Log ("RotateRight");
		}
	}

	public void RotateUp()
	{
		if (!m_isRotating && !m_rotationBlocked)
		{
			Rotate90Degrees(1, Axis.X, 0.3f);
			m_cubeState.DoRotation(CubeState.RotationActions.UP);
			m_lastRotation = CubeState.RotationActions.UP;
			Debug.Log ("RotateUp");
		}
	}

	public void RotateDown()
	{
		if (!m_isRotating && !m_rotationBlocked)
		{
			Rotate90Degrees(-1, Axis.X, 0.3f);
			m_cubeState.DoRotation(CubeState.RotationActions.DOWN);
			m_lastRotation = CubeState.RotationActions.DOWN;
			Debug.Log ("RotateDown");
		}
	}

	public void RotateClockwise()
	{
		if (!m_isRotating && !m_rotationBlocked)
		{
			Rotate90Degrees(-1, Axis.Z, 0.3f);
			m_cubeState.DoRotation(CubeState.RotationActions.CLOCKWISE);
			m_lastRotation = CubeState.RotationActions.CLOCKWISE;
		}
	}

	public void RotateAntiClockwise()
	{
		if (!m_isRotating && !m_rotationBlocked)
		{
			Rotate90Degrees(1, Axis.Z, 0.3f);
			m_cubeState.DoRotation(CubeState.RotationActions.ANTI_CLOCKWISE);
			m_lastRotation = CubeState.RotationActions.ANTI_CLOCKWISE;
		}
	}

	public void BlockRotationForTimePeriod(float time)
	{
		StartCoroutine(BlockRotationForTimePeriodCoroutine(time));
	}

	IEnumerator BlockRotationForTimePeriodCoroutine(float time)
	{
		m_rotationBlocked = true;
		yield return new WaitForSeconds(time);
		m_rotationBlocked = false;
	}
}
