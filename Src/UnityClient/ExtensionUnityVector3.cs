using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    public static class ExtensionUnityVector3
    {
        public static bool IsNearlyZero(this Vector3 vector)
        {
            return Mathf.Approximately(vector.sqrMagnitude, 0);
        }
        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
        }
        public static float ToDir(this Vector3 vector)
        {
            return Mathf.Atan2(vector.x, vector.z);
        }
    }
}
