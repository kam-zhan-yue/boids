using UnityEngine;

public static class GizmosExtensions
{
    /// <summary>
    /// Draws a wire arc.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dir">The direction from which the anglesRange is taken into account</param>
    /// <param name="anglesRange">The angle range, in degrees.</param>
    /// <param name="radius"></param>
    /// <param name="maxSteps">How many steps to use to draw the arc.</param>
    public static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
    {
        float srcAngles = GetAnglesFromDir(position, dir);
        Vector3 initialPos = position;
        Vector3 posA = initialPos;
        float stepAngles = anglesRange / maxSteps;
        float angle = srcAngles - anglesRange / 2;
        for (int i = 0; i <= maxSteps; i++)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector3 posB = initialPos;
            posB += new Vector3(radius * Mathf.Sin(rad), radius * Mathf.Cos(rad), 0f);

            Gizmos.DrawLine(posA, posB);

            angle += stepAngles;
            posA = posB;
        }
        Gizmos.DrawLine(posA, initialPos);
    }

    private static float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        return BoidExtensions.GetAngle(dir);
        Vector3 forwardLimitPos = position + dir;
        float srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return srcAngles;
    }
}