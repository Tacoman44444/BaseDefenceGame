using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonDamageScript : MonoBehaviour
{
    public CannonStats cannonStats;

    private void Start()
    {
        cannonStats = GetComponent<CannonStats>();
        cannonStats.ApplyDamage(10);
    }

    private void OnTriggerEnter2D(Collider2D other)     //set bullet colliders to IsTrigger
    {
        if (other.CompareTag("gunner"))
        {
            cannonStats.ApplyDamage(10);
        }

        /*
        if (other.CompareTag("sniper"))
        {
            cannonStats.ApplyDamage(32);
        }
        */

    }
}
