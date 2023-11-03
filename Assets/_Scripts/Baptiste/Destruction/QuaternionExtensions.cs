using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionExtensions
{
    public static Vector4 ToVector4(this Quaternion quaternion)
    {
        return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }

    public static Quaternion ToQuaternion(this Vector4 vector)
    {
        return new Quaternion(vector.x, vector.y, vector.z, vector.w);
    }
}
