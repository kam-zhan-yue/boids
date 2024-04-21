using UnityEngine;

public static class BoidExtensions
{
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

    public static float AddAngle(float original, float addition)
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

    public static Vector3 GetAttractiveForce(Vector3 pos1, Vector3 pos2, float radius)
    {
        // Get the difference between the positions
        Vector3 difference = pos2 - pos1;
        // Calculate a ratio based on relative distance in regards to the radius
        // return difference.normalized / difference.sqrMagnitude;
        float radio = Mathf.Clamp01(difference.magnitude / radius);
        return radio * difference;
    }

}