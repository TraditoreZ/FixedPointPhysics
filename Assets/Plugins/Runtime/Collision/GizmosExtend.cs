using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosExtend
{
    public static void DrawWireCube(Vector3 position, Quaternion rotation, Vector3 size)
    {
        Vector3 AxisX = rotation * Vector3.right;
        Vector3 AxisY = rotation * Vector3.up;
        Vector3 AxisZ = rotation * Vector3.forward;
        Vector3 extents = size * 0.5f;
        Vector3 x = AxisX * extents.x;
        Vector3 y = AxisY * extents.y;
        Vector3 z = AxisZ * extents.z;

        Vector3 p1 = position + x + y + z;
        Vector3 p2 = position - x + y + z;
        Vector3 p3 = position + x - y + z;
        Vector3 p4 = position + x + y - z;
        Vector3 p5 = position + x - y - z;
        Vector3 p6 = position - x - y + z;
        Vector3 p7 = position - x + y - z;
        Vector3 p8 = position - x - y - z;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p6);
        Gizmos.DrawLine(p6, p3);
        Gizmos.DrawLine(p3, p1);

        Gizmos.DrawLine(p4, p7);
        Gizmos.DrawLine(p7, p8);
        Gizmos.DrawLine(p8, p5);
        Gizmos.DrawLine(p5, p4);

        Gizmos.DrawLine(p1, p4);
        Gizmos.DrawLine(p2, p7);
        Gizmos.DrawLine(p3, p5);
        Gizmos.DrawLine(p6, p8);
    }

    public static void DrawWireCapsule(Vector3 center, Quaternion rotation, float radius, float height)
    {
        Vector3 AxisX = rotation * Vector3.right;
        Vector3 AxisY = rotation * Vector3.up;
        Vector3 AxisZ = rotation * Vector3.forward;
        float s = Mathf.Max(height * .5f - radius, 0);

        Vector3 x = AxisX * radius;
        Vector3 y = AxisY * s;
        Vector3 z = AxisZ * radius;

        Vector3 p1 = center + x + y;
        Vector3 p2 = center - x + y;
        Vector3 p4 = center + y + z;
        Vector3 p7 = center + y - z;

        Vector3 p3 = center + x - y;
        Vector3 p5 = center - x - y;
        Vector3 p6 = center - y + z;
        Vector3 p8 = center - y - z;

        Gizmos.DrawWireSphere(center + AxisY * s, radius);
        Gizmos.DrawWireSphere(center - AxisY * s, radius);
        Gizmos.DrawLine(p1, p3);
        Gizmos.DrawLine(p2, p5);
        Gizmos.DrawLine(p4, p6);
        Gizmos.DrawLine(p7, p8);
    }



}
