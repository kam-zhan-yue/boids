using UnityEngine;

public struct BoidData
{
    public Vector3 position;
    public Vector3 direction;
    public Vector3 separationForce;
    public Vector3 alignmentForce;
    public Vector3 cohesionForce;
    public Vector3 avoidanceForce;
    public int groupId;
    public uint predator;

    public static int GetStrideLength()
    {
        int positionSize = sizeof(float) * 3;
        int directionSize = sizeof(float) * 3;
        int separationSize = sizeof(float) * 3;
        int alignmentSize = sizeof(float) * 3;
        int cohesionSize = sizeof(float) * 3;
        int avoidanceSize = sizeof(float) * 3;
        int groupSize = sizeof(int);
        int predatorSize = sizeof(uint);
        return positionSize + directionSize + separationSize + alignmentSize + cohesionSize + avoidanceSize + groupSize + predatorSize;
    }
}