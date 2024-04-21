using UnityEngine;

public static class BoidExtensions
{
    public static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Offset angle
        angle -= 90f;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }
}