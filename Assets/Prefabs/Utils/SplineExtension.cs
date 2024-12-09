using UnityEngine;
using UnityEngine.Splines;

// Based on the code by Brogan89
// https://discussions.unity.com/t/how-to-move-an-object-along-a-spline-spriteshape/778762/10

public static class SplineEx
{
    /// <summary>
    /// Returns the local position along the spline based on progress 0 - 1.
    /// Good for lerping an object along the spline.
    /// <para></para>
    /// Example: transform.localPosition = spline.GetPoint(0.5f)
    /// </summary>
    /// <param name="spline"></param>
    /// <param name="progress">Value from 0 - 1</param>
    /// <returns></returns>
    public static void GetPoint(this Spline spline, float progress, out Vector3 position, out Vector3 tangent)
    {
        var length = spline.Count;
        var i = Mathf.Clamp(Mathf.CeilToInt((length - 1) * progress), 0, length - 1);

        var t = progress * (length - 1) % 1f;
        if (i == length - 1 && progress >= 1f)
            t = 1;

        var prevIndex = Mathf.Max(i - 1, 0);

        var prevVertex = spline[prevIndex];
        var ithVertex = spline[i];
        Vector3 p0 = prevVertex.Position;
        Vector3 p3 = ithVertex.Position;
        Vector3 tangentOut = prevVertex.TangentOut;
        Quaternion prevQuat = prevVertex.Rotation;
        var p1 = p0 + prevQuat * tangentOut;
        Vector3 tangentIn = ithVertex.TangentIn;
        Quaternion ithQuat = ithVertex.Rotation;
        var p2 = p3 + ithQuat * tangentIn;

        var a0 = Vector3.Lerp(p0, p1, t);
        var a1 = Vector3.Lerp(p1, p2, t);
        var a2 = Vector3.Lerp(p2, p3, t);

        var b0 = Vector3.Lerp(a0, a1, t);
        var b1 = Vector3.Lerp(a1, a2, t);

        position = Vector3.Lerp(b0, b1, t);
        tangent = b1 - b0;
    }
}