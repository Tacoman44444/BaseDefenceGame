using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class GameStateInformation
{

    public static Hashtable damagedWalls = new Hashtable();

    public static List<Transform> deployedCannons = new List<Transform>();

    public static float cannondetectedRadius = 2;

}
