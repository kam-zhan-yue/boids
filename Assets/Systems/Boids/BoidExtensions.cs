using UnityEngine;

public static class BoidExtensions
{
    const int NUM_VIEW_DIRECTIONS = 300;
    public static readonly Vector3[] Directions;
    public static readonly float GoldenRatio = (1 + Mathf.Sqrt(5)) / 2;

    static BoidExtensions () {
        Directions = new Vector3[NUM_VIEW_DIRECTIONS];
        CalculateSpherePoints(Directions, GoldenRatio);
    }

    public static void CalculateSpherePoints(Vector3[] directions, float ratio)
    {
        float angleIncrement = Mathf.PI * 2 * ratio;

        for (int i = 0; i < directions.Length; ++i) {
            float t = (float) i / directions.Length;
            float inclination = Mathf.Acos (1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            float z = Mathf.Cos (inclination);
            directions[i] = new Vector3 (x, y, z);
        }
    }
    
    public static float GetTransformAngle(Vector2 direction)
    {
        float angle = GetAngle(direction);
        // Offset angle
        angle = AddAngle(angle, -90f);
        return angle;
    }

    public static float GetAngle(Vector2 direction)
    {
        return ClampAngle(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    private static float AddAngle(float original, float addition)
    {
        original += addition;
        return ClampAngle(original);
    }

    private static float ClampAngle(float angle)
    {
        if (angle < 0f)
            angle += 360f;
        else if (angle > 360f)
            angle -= 360f;
        return angle;
    }
}