using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float movementSpeed = 1f; // Adjust this value to control the speed of movement
        Vector3 movement = new Vector3(-1, 0, 0) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);

    }
}
