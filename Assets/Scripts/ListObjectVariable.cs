using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObject/List GameObject Variable")]
public class ListObjectVariable : ScriptableObject
{
    public List<Transform> enemies = new List<Transform>();
}
