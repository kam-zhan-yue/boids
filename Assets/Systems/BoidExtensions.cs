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
}