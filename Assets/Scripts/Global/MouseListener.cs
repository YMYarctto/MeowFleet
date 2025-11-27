using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseListener : MonoBehaviour
{
    Vector3 lastMousePos;

    public static float MouseSpeed;
    public static bool MouseMove;
    public static Vector3 MousePosition;

    void FixedUpdate()
    {
        MousePosition = Input.mousePosition;
        MouseSpeed = (MousePosition - lastMousePos).magnitude;
        lastMousePos = MousePosition;
        MouseMove = MouseSpeed >= 1f;
    }

    public static float DistanceY(Vector3 pos)
    {
        return pos.y - MousePosition.y;
    }

    public static float Distance(Vector3 pos)
    {
        return (pos - MousePosition).magnitude;
    }
}
