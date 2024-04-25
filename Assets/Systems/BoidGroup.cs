using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/BoidGroup")]
public class BoidGroup : ScriptableObject
{
    public bool predator;
    public Color color;
    public Material material;
}
