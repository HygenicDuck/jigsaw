using UnityEngine;
using System.Collections;

[System.Serializable]
public class FaceState
{
	// orientations are all defined relative to a viewer that rotates the shortest way to the specified face.
	// i.e. 
	// front face - viewer is facing forward, with up vector pointing up
	// left face - viewer is looking to the right, with up vector pointing up
	// right face - viewer is looking to the left, with up vector pointing up
	// bottom face - viewer is looking up, with up vector pointing towards the camera
	// top face - viewer is looking down, with up vector pointing away from the camera
	// back face - this is the tricky one, since there are two ways to rotate to the back face. 
	// We will define it as viewer is looking towards the camera, with up vector pointing down
	public enum Orientations
	{
		UP = 0,
		RIGHT,
		DOWN,
		LEFT,
		NUM_ORIENTATIONS
	}

	[SerializeField] 
	public int m_state;
	[SerializeField] 
	public Orientations m_orientation;

    public FaceState(int state, Orientations orientation)
    {
        m_state = state;
        m_orientation = orientation;
    }
}

[System.Serializable]
public class FaceStates
{
    [SerializeField]
    public FaceState[] m_faceStates = new FaceState[(int)CubeState.Faces.NUM_FACES];

    public FaceStates(FaceState front, FaceState top, FaceState right, FaceState bottom, FaceState left, FaceState back)
    {
        m_faceStates[(int)CubeState.Faces.FRONT] = front;
        m_faceStates[(int)CubeState.Faces.TOP] = top;
        m_faceStates[(int)CubeState.Faces.RIGHT] = right;
        m_faceStates[(int)CubeState.Faces.BOTTOM] = bottom;
        m_faceStates[(int)CubeState.Faces.LEFT] = left;
        m_faceStates[(int)CubeState.Faces.BACK] = back;
    }

    public FaceState GetFaceState(CubeState.Faces face)
    {
        return m_faceStates[(int)face];
    }
}

public class CubeState : MonoBehaviour
{
	public enum Faces
	{
		FRONT = 0,
		TOP,
		RIGHT,
		BOTTOM,
		LEFT,
		BACK,
		NUM_FACES
	};

	public enum RotationActions
	{
		UP = 0,
		DOWN,
		LEFT,
		RIGHT,
		CLOCKWISE,
		ANTI_CLOCKWISE,
		NONE
	};
		
	public static RotationActions[] m_flipHorizontally = { RotationActions.UP, RotationActions.DOWN, RotationActions.RIGHT, RotationActions.LEFT, RotationActions.ANTI_CLOCKWISE, RotationActions.CLOCKWISE };
	public static RotationActions[] m_flipVertically = { RotationActions.DOWN, RotationActions.UP, RotationActions.LEFT, RotationActions.RIGHT, RotationActions.ANTI_CLOCKWISE, RotationActions.CLOCKWISE };
		
	[SerializeField] 
	FaceState[] m_faceStates = new FaceState[(int)Faces.NUM_FACES];


	public void Init()
	{
//		m_faceStates[(int)Faces.FRONT].state = 6;
//		m_faceStates[(int)Faces.TOP].state = 5;
//		m_faceStates[(int)Faces.RIGHT].state = 4;
//		m_faceStates[(int)Faces.LEFT].state = 3;
//		m_faceStates[(int)Faces.BOTTOM].state = 2;
//		m_faceStates[(int)Faces.BACK].state = 1;

		m_faceStates[(int)Faces.FRONT].m_state = 0;
		m_faceStates[(int)Faces.TOP].m_state = 1;
		m_faceStates[(int)Faces.RIGHT].m_state = 2;
		m_faceStates[(int)Faces.BOTTOM].m_state = 3;
		m_faceStates[(int)Faces.LEFT].m_state = 4;
		m_faceStates[(int)Faces.BACK].m_state = 5;
	}

    public bool CheckState(FaceStates states)
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_faceStates[i].m_state != states.m_faceStates[i].m_state)
                return false;
            if (m_faceStates[i].m_orientation != states.m_faceStates[i].m_orientation)
                return false;
        }
        return true;
    }

    public static Faces FaceFromVector(Vector3 vec)
    {
        if (vec.z == 0f)
        {
            if (vec.y == 0f)
            {
                if (vec.x >= 0f)
                {
                    return Faces.RIGHT;
                }
                else
                {
                    return Faces.LEFT;
                }
            }
            else
            {
                if (vec.y >= 0f)
                {
                    return Faces.TOP;
                }
                else
                {
                    return Faces.BOTTOM;
                }
            }
        }

        if (vec.z >= 0f)
        {
            return Faces.BACK;
        }
        else
        {
            return Faces.FRONT;
        }
    }

    void AdjustOrientation(Faces face, int adjustment)
	{
		int rot = (int)m_faceStates[(int)face].m_orientation;
		rot += adjustment;
		if (rot < 0) rot += (int)FaceState.Orientations.NUM_ORIENTATIONS;
		rot = rot % (int)FaceState.Orientations.NUM_ORIENTATIONS;
		m_faceStates[(int)face].m_orientation = (FaceState.Orientations)rot;
	}

	public void DoRotation(RotationActions rot)
	{
		FaceState temp;

		switch(rot)
		{
		case RotationActions.UP:
			Debug.Log("RotationActions.UP");
			temp = m_faceStates[(int)Faces.FRONT];
			m_faceStates[(int)Faces.FRONT] = m_faceStates[(int)Faces.BOTTOM];
			m_faceStates[(int)Faces.BOTTOM] = m_faceStates[(int)Faces.BACK];
			m_faceStates[(int)Faces.BACK] = m_faceStates[(int)Faces.TOP];
			m_faceStates[(int)Faces.TOP] = temp;
			AdjustOrientation(Faces.LEFT, -1);
			AdjustOrientation(Faces.RIGHT, 1);
			break;
		case RotationActions.DOWN:
			Debug.Log("RotationActions.DOWN");
			temp = m_faceStates[(int)Faces.FRONT];
			m_faceStates[(int)Faces.FRONT] = m_faceStates[(int)Faces.TOP];
			m_faceStates[(int)Faces.TOP] = m_faceStates[(int)Faces.BACK];
			m_faceStates[(int)Faces.BACK] = m_faceStates[(int)Faces.BOTTOM];
			m_faceStates[(int)Faces.BOTTOM] = temp;
			AdjustOrientation(Faces.LEFT, 1);
			AdjustOrientation(Faces.RIGHT, -1);
			break;
		case RotationActions.LEFT:
			Debug.Log("RotationActions.LEFT");
			temp = m_faceStates[(int)Faces.FRONT];
			m_faceStates[(int)Faces.FRONT] = m_faceStates[(int)Faces.RIGHT];
			m_faceStates[(int)Faces.RIGHT] = m_faceStates[(int)Faces.BACK];
			m_faceStates[(int)Faces.BACK] = m_faceStates[(int)Faces.LEFT];
			m_faceStates[(int)Faces.LEFT] = temp;
			AdjustOrientation(Faces.TOP, 1);
			AdjustOrientation(Faces.BOTTOM, -1);
			AdjustOrientation(Faces.BACK, 2);
			AdjustOrientation(Faces.RIGHT, 2);
			break;
		case RotationActions.RIGHT:
			Debug.Log("RotationActions.RIGHT");
			temp = m_faceStates[(int)Faces.FRONT];
			m_faceStates[(int)Faces.FRONT] = m_faceStates[(int)Faces.LEFT];
			m_faceStates[(int)Faces.LEFT] = m_faceStates[(int)Faces.BACK];
			m_faceStates[(int)Faces.BACK] = m_faceStates[(int)Faces.RIGHT];
			m_faceStates[(int)Faces.RIGHT] = temp;
			AdjustOrientation(Faces.TOP, -1);
			AdjustOrientation(Faces.BOTTOM, 1);
			AdjustOrientation(Faces.BACK, 2);
			AdjustOrientation(Faces.LEFT, 2);
			break;
		case RotationActions.CLOCKWISE:
			Debug.Log("RotationActions.CLOCKWISE");
			temp = m_faceStates[(int)Faces.TOP];
			m_faceStates[(int)Faces.TOP] = m_faceStates[(int)Faces.LEFT];
			m_faceStates[(int)Faces.LEFT] = m_faceStates[(int)Faces.BOTTOM];
			m_faceStates[(int)Faces.BOTTOM] = m_faceStates[(int)Faces.RIGHT];
			m_faceStates[(int)Faces.RIGHT] = temp;
			AdjustOrientation(Faces.FRONT, 1);
			AdjustOrientation(Faces.BACK, -1);
			AdjustOrientation(Faces.TOP, 1);
			AdjustOrientation(Faces.BOTTOM, 1);
			AdjustOrientation(Faces.LEFT, 1);
			AdjustOrientation(Faces.RIGHT, 1);
			break;
		case RotationActions.ANTI_CLOCKWISE:
			Debug.Log("RotationActions.ANTI_CLOCKWISE");
			temp = m_faceStates[(int)Faces.TOP];
			m_faceStates[(int)Faces.TOP] = m_faceStates[(int)Faces.RIGHT];
			m_faceStates[(int)Faces.RIGHT] = m_faceStates[(int)Faces.BOTTOM];
			m_faceStates[(int)Faces.BOTTOM] = m_faceStates[(int)Faces.LEFT];
			m_faceStates[(int)Faces.LEFT] = temp;
			AdjustOrientation(Faces.FRONT, -1);
			AdjustOrientation(Faces.BACK, 1);
			AdjustOrientation(Faces.TOP, -1);
			AdjustOrientation(Faces.BOTTOM, -1);
			AdjustOrientation(Faces.LEFT, -1);
			AdjustOrientation(Faces.RIGHT, -1);
			break;

		}
	}

	public FaceState GetFaceState(Faces face)
	{
		return m_faceStates[(int)face];
	}

	public void SetFaceState(Faces face, FaceState state)
	{
		m_faceStates[(int)face] = state;
	}

	public void PrintState()
	{
		for(int i=0; i<(int)Faces.NUM_FACES; i++)
		{
			FaceState state = GetFaceState((Faces)i);
			Debug.Log("Face "+i+" = "+state.m_state);
		}
	}
		
}
