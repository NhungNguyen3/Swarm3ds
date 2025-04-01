using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Weapon 
{
    public string id;
    public string name;
    public Sprite icon;
    public GameObject model;
    public float attackSpeed;
    public float baseDmg;
    public float reloadTime;
    public int ammoUsable;
    public int ammoStorage;
}
