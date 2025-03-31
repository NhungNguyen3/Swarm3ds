using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/List GameObject Variable")]
public class ListObjectVariable : ScriptableObject
{
    public List<Enemy> enemies = new List<Enemy>();
}
