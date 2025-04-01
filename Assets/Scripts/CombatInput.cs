using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CombatInput 
{
    public GameObject map;
    public gameMode gameMode;
    public CharacterInformation chartacterInfor; // attribue, model, 
    public List<Weapon> guns;
    public Drone drone;

    // public LevelData leveldata;

    
}


public enum gameMode
{
    Campagne,
    Endless
}