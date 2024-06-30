using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonStats : MonoBehaviour
{
    [SerializeField]
    private float health = 250;

    // Getter method for accessing health from other scripts
    public float Health
    {
        get { return health; }
    }

    // Method to apply damage to the cannon's health
    public void ApplyDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            GameStateInformation.deployedCannons.Remove(gameObject.transform);
            // Handle destruction of the cannon if health reaches zero
            Destroy(gameObject);
        }
    }
}
