using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyType")]
public class EnemyTypeSO : ScriptableObject 
{
    public string nameString;
    public float damageToCannon = 10;
    public float damageToPirate;
    public float damageToWall;
    public float range = 2;     
    public float speed;
    public Transform prefab;
    public float projectileSpeed = 10.0f;
    public float rateOfFire = 1.0f;
}


