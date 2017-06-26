using UnityEngine;
using System.Collections;

public class CubeOrientation : MonoBehaviour
{
    private static Vector3 NearestWorldAxis(Vector3 v)
    {
        if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
        {
            v.x = 0;
            if (Mathf.Abs(v.y) < Mathf.Abs(v.z))
                v.y = 0;
            else
                v.z = 0;
        }
        else
        {
            v.y = 0;
            if (Mathf.Abs(v.x) < Mathf.Abs(v.z))
                v.x = 0;
            else
                v.z = 0;
        }
        return v;
    }

    public Quaternion Closest90DegreePosition()
    {
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;
        //Debug.Log("forward = "+ up.x + ", "+ up.y + ", "+ up.z);
        Vector3 alignedForward = NearestWorldAxis(transform.forward);
        Vector3 alignedUp = NearestWorldAxis(transform.up);
        Quaternion quat = Quaternion.LookRotation(alignedForward, alignedUp);

        return quat;
    }

    private void Update()
    {
        //Vector3 forward = transform.forward;
        //Vector3 up = transform.up;
        ////Debug.Log("forward = "+ up.x + ", "+ up.y + ", "+ up.z);
        //Vector3 alignedForward = NearestWorldAxis(transform.forward);
        //Vector3 alignedUp = NearestWorldAxis(transform.up);
        ////transform.rotation = Quaternion.LookRotation(alignedForward, alignedUp);
        //Debug.Log("forward = " + alignedForward.x + ", " + alignedForward.y + ", " + alignedForward.z);
    }


    public FaceStates GetFaceStates()
    {
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;

        // negate the front vector since I consider it coming towards the camera.
        Vector3 alignedForward = -NearestWorldAxis(transform.forward);
        Vector3 alignedUp = NearestWorldAxis(transform.up);

        CubeState.Faces forwardFace = CubeState.FaceFromVector(alignedForward);
        CubeState.Faces upFace = CubeState.FaceFromVector(alignedUp);

        switch(forwardFace)
        {
            case CubeState.Faces.FRONT:
                switch (upFace)
                { 
                    case CubeState.Faces.FRONT:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.BACK:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.TOP:
                        return new FaceStates(
                            new FaceState(0, FaceState.Orientations.UP),
                            new FaceState(1, FaceState.Orientations.UP),
                            new FaceState(2, FaceState.Orientations.UP),
                            new FaceState(3, FaceState.Orientations.UP),
                            new FaceState(4, FaceState.Orientations.UP),
                            new FaceState(5, FaceState.Orientations.UP)
                            );
                        break;
                    case CubeState.Faces.BOTTOM:
                        return new FaceStates(
                            new FaceState(0, FaceState.Orientations.DOWN),
                            new FaceState(3, FaceState.Orientations.DOWN),
                            new FaceState(4, FaceState.Orientations.DOWN),
                            new FaceState(1, FaceState.Orientations.DOWN),
                            new FaceState(2, FaceState.Orientations.DOWN),
                            new FaceState(5, FaceState.Orientations.DOWN)
                            );
                        break;
                    case CubeState.Faces.LEFT:
                        return new FaceStates(
                            new FaceState(0, FaceState.Orientations.LEFT),
                            new FaceState(2, FaceState.Orientations.LEFT),
                            new FaceState(3, FaceState.Orientations.LEFT),
                            new FaceState(4, FaceState.Orientations.LEFT),
                            new FaceState(1, FaceState.Orientations.LEFT),
                            new FaceState(5, FaceState.Orientations.RIGHT)
                            );
                        break;
                    case CubeState.Faces.RIGHT:
                        return new FaceStates(
                            new FaceState(0, FaceState.Orientations.RIGHT),
                            new FaceState(4, FaceState.Orientations.RIGHT),
                            new FaceState(1, FaceState.Orientations.RIGHT),
                            new FaceState(2, FaceState.Orientations.RIGHT),
                            new FaceState(3, FaceState.Orientations.RIGHT),
                            new FaceState(5, FaceState.Orientations.LEFT)
                            );
                        break;
                }
                break;
            case CubeState.Faces.BACK:
                switch (upFace)
                {
                    case CubeState.Faces.FRONT:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.BACK:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.TOP:
                        return new FaceStates(
                            new FaceState(5, FaceState.Orientations.DOWN),
                            new FaceState(1, FaceState.Orientations.DOWN),
                            new FaceState(4, FaceState.Orientations.UP),
                            new FaceState(3, FaceState.Orientations.DOWN),
                            new FaceState(2, FaceState.Orientations.UP),
                            new FaceState(0, FaceState.Orientations.DOWN)
                            );
                        break;
                    case CubeState.Faces.BOTTOM:
                        return new FaceStates(
                            new FaceState(5, FaceState.Orientations.UP),
                            new FaceState(3, FaceState.Orientations.UP),
                            new FaceState(2, FaceState.Orientations.DOWN),
                            new FaceState(1, FaceState.Orientations.UP),
                            new FaceState(4, FaceState.Orientations.DOWN),
                            new FaceState(0, FaceState.Orientations.UP)
                            );
                        break;
                    case CubeState.Faces.LEFT:
                        return new FaceStates(
                            new FaceState(5, FaceState.Orientations.RIGHT),
                            new FaceState(4, FaceState.Orientations.LEFT),
                            new FaceState(3, FaceState.Orientations.RIGHT),
                            new FaceState(2, FaceState.Orientations.LEFT),
                            new FaceState(1, FaceState.Orientations.RIGHT),
                            new FaceState(0, FaceState.Orientations.LEFT)
                            );
                        break;
                    case CubeState.Faces.RIGHT:
                        return new FaceStates(
                            new FaceState(5, FaceState.Orientations.LEFT),
                            new FaceState(2, FaceState.Orientations.RIGHT),
                            new FaceState(1, FaceState.Orientations.LEFT),
                            new FaceState(4, FaceState.Orientations.RIGHT),
                            new FaceState(3, FaceState.Orientations.LEFT),
                            new FaceState(0, FaceState.Orientations.RIGHT)
                            );
                        break;
                }
                break;
            case CubeState.Faces.TOP:
                switch (upFace)
                {
                    case CubeState.Faces.FRONT:
                        return new FaceStates(
                           new FaceState(1, FaceState.Orientations.DOWN),
                           new FaceState(0, FaceState.Orientations.DOWN),
                           new FaceState(4, FaceState.Orientations.LEFT),
                           new FaceState(5, FaceState.Orientations.DOWN),
                           new FaceState(2, FaceState.Orientations.RIGHT),
                           new FaceState(3, FaceState.Orientations.DOWN)
                           );
                        break;
                    case CubeState.Faces.BACK:
                        return new FaceStates(
                            new FaceState(3, FaceState.Orientations.UP),
                            new FaceState(0, FaceState.Orientations.UP),
                            new FaceState(2, FaceState.Orientations.RIGHT),
                            new FaceState(5, FaceState.Orientations.UP),
                            new FaceState(4, FaceState.Orientations.LEFT),
                            new FaceState(1, FaceState.Orientations.UP)
                            );
                        break;
                    case CubeState.Faces.TOP:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.BOTTOM:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.LEFT:
                        return new FaceStates(
                           new FaceState(4, FaceState.Orientations.LEFT),
                           new FaceState(0, FaceState.Orientations.LEFT),
                           new FaceState(3, FaceState.Orientations.UP),
                           new FaceState(5, FaceState.Orientations.RIGHT),
                           new FaceState(1, FaceState.Orientations.DOWN),
                           new FaceState(2, FaceState.Orientations.LEFT)
                           );
                        break;
                    case CubeState.Faces.RIGHT:
                        return new FaceStates(
                           new FaceState(2, FaceState.Orientations.RIGHT),
                           new FaceState(0, FaceState.Orientations.RIGHT),
                           new FaceState(1, FaceState.Orientations.DOWN),
                           new FaceState(5, FaceState.Orientations.LEFT),
                           new FaceState(3, FaceState.Orientations.UP),
                           new FaceState(4, FaceState.Orientations.RIGHT)
                           );
                        break;
                }
                break;
            case CubeState.Faces.BOTTOM:
                switch (upFace)
                {
                    case CubeState.Faces.FRONT:
                        return new FaceStates(
                           new FaceState(1, FaceState.Orientations.UP),
                           new FaceState(5, FaceState.Orientations.UP),
                           new FaceState(2, FaceState.Orientations.LEFT),
                           new FaceState(0, FaceState.Orientations.UP),
                           new FaceState(4, FaceState.Orientations.RIGHT),
                           new FaceState(3, FaceState.Orientations.UP)
                           );
                        break;
                    case CubeState.Faces.BACK:
                        return new FaceStates(
                           new FaceState(3, FaceState.Orientations.DOWN),
                           new FaceState(5, FaceState.Orientations.DOWN),
                           new FaceState(4, FaceState.Orientations.RIGHT),
                           new FaceState(0, FaceState.Orientations.DOWN),
                           new FaceState(2, FaceState.Orientations.LEFT),
                           new FaceState(1, FaceState.Orientations.DOWN)
                           );
                        break;
                    case CubeState.Faces.TOP:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.BOTTOM:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.LEFT:
                        return new FaceStates(
                           new FaceState(2, FaceState.Orientations.LEFT),
                           new FaceState(5, FaceState.Orientations.RIGHT),
                           new FaceState(3, FaceState.Orientations.DOWN),
                           new FaceState(0, FaceState.Orientations.LEFT),
                           new FaceState(1, FaceState.Orientations.UP),
                           new FaceState(4, FaceState.Orientations.LEFT)
                           );
                        break;
                    case CubeState.Faces.RIGHT:
                        return new FaceStates(
                           new FaceState(4, FaceState.Orientations.RIGHT),
                           new FaceState(5, FaceState.Orientations.LEFT),
                           new FaceState(1, FaceState.Orientations.UP),
                           new FaceState(0, FaceState.Orientations.RIGHT),
                           new FaceState(3, FaceState.Orientations.DOWN),
                           new FaceState(2, FaceState.Orientations.RIGHT)
                           );
                        break;
                }
                break;
            case CubeState.Faces.LEFT:
                switch (upFace)
                {
                    case CubeState.Faces.FRONT:
                        return new FaceStates(
                           new FaceState(1, FaceState.Orientations.RIGHT),
                           new FaceState(4, FaceState.Orientations.DOWN),
                           new FaceState(5, FaceState.Orientations.RIGHT),
                           new FaceState(2, FaceState.Orientations.UP),
                           new FaceState(0, FaceState.Orientations.RIGHT),
                           new FaceState(3, FaceState.Orientations.LEFT)
                           );
                        break;
                    case CubeState.Faces.BACK:
                        return new FaceStates(
                           new FaceState(3, FaceState.Orientations.LEFT),
                           new FaceState(2, FaceState.Orientations.UP),
                           new FaceState(5, FaceState.Orientations.LEFT),
                           new FaceState(4, FaceState.Orientations.DOWN),
                           new FaceState(0, FaceState.Orientations.LEFT),
                           new FaceState(1, FaceState.Orientations.RIGHT)
                           );
                        break;
                    case CubeState.Faces.TOP:
                        return new FaceStates(
                           new FaceState(2, FaceState.Orientations.UP),
                           new FaceState(1, FaceState.Orientations.RIGHT),
                           new FaceState(5, FaceState.Orientations.DOWN),
                           new FaceState(3, FaceState.Orientations.LEFT),
                           new FaceState(0, FaceState.Orientations.UP),
                           new FaceState(4, FaceState.Orientations.DOWN)
                           );
                        break;
                    case CubeState.Faces.BOTTOM:
                        return new FaceStates(
                           new FaceState(4, FaceState.Orientations.DOWN),
                           new FaceState(3, FaceState.Orientations.LEFT),
                           new FaceState(5, FaceState.Orientations.UP),
                           new FaceState(1, FaceState.Orientations.RIGHT),
                           new FaceState(0, FaceState.Orientations.DOWN),
                           new FaceState(2, FaceState.Orientations.UP)
                           );
                        break;
                    case CubeState.Faces.LEFT:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.RIGHT:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                }
                break;
            case CubeState.Faces.RIGHT:
                switch (upFace)
                {
                    case CubeState.Faces.FRONT:
                        return new FaceStates(
                           new FaceState(1, FaceState.Orientations.LEFT),
                           new FaceState(2, FaceState.Orientations.DOWN),
                           new FaceState(0, FaceState.Orientations.LEFT),
                           new FaceState(4, FaceState.Orientations.UP),
                           new FaceState(5, FaceState.Orientations.LEFT),
                           new FaceState(3, FaceState.Orientations.RIGHT)
                           );
                        break;
                    case CubeState.Faces.BACK:
                        return new FaceStates(
                           new FaceState(3, FaceState.Orientations.RIGHT),
                           new FaceState(4, FaceState.Orientations.UP),
                           new FaceState(0, FaceState.Orientations.RIGHT),
                           new FaceState(2, FaceState.Orientations.DOWN),
                           new FaceState(5, FaceState.Orientations.RIGHT),
                           new FaceState(1, FaceState.Orientations.LEFT)
                           );
                        break;
                    case CubeState.Faces.TOP:
                        return new FaceStates(
                           new FaceState(4, FaceState.Orientations.UP),
                           new FaceState(1, FaceState.Orientations.LEFT),
                           new FaceState(0, FaceState.Orientations.UP),
                           new FaceState(3, FaceState.Orientations.RIGHT),
                           new FaceState(5, FaceState.Orientations.DOWN),
                           new FaceState(2, FaceState.Orientations.DOWN)
                           );
                        break;
                    case CubeState.Faces.BOTTOM:
                        return new FaceStates(
                           new FaceState(2, FaceState.Orientations.DOWN),
                           new FaceState(3, FaceState.Orientations.RIGHT),
                           new FaceState(0, FaceState.Orientations.DOWN),
                           new FaceState(1, FaceState.Orientations.LEFT),
                           new FaceState(5, FaceState.Orientations.UP),
                           new FaceState(4, FaceState.Orientations.UP)
                           );
                        break;
                    case CubeState.Faces.LEFT:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                    case CubeState.Faces.RIGHT:
                        Debug.LogError("IMPOSSIBLE ORIENTATION");
                        break;
                }
                break;
        }

        // couldn't find a valid orientation
        return null;
    }
}
