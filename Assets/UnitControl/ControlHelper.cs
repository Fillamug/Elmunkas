using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ControlHelper
{
    public static Vector3 GetMouseWorldPos()
    {
        Vector3 worldPos;
        RaycastHit raycastHit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, Mathf.Infinity))
        {
            worldPos = raycastHit.point;
        }
        else worldPos = Vector3.zero;
        return worldPos;
    }

    public static Vector3 GetMouseViewportPos()
    {
        Vector3 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        return viewportPos;
    }
}
