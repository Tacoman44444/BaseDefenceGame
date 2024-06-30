using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GunnerMoveScript : MonoBehaviour     //Rename to GunnerMoveAI
{
    // Start is called before the first frame update
    public EnemyTypeSO gunner;
    [SerializeField] private float cannonDetectRange;
    float gunnerRange;
    public Tilemap fortTiles;
    public float test_increment_for_line_of_sight;  //remove after finalizing
    GameObject selectedCannon;
    public Transform sampleCannon;     //remove after testing
    public Transform sampleCannon2;    //remove after testing
    public Transform sampleCannon3;    //remove after testing
    public Transform bulletPrefab;
    private bool isFiring;
    private Coroutine firingCoroutine;
    public float SAMPLE_ROF = 1.0f;
    public float SAMPLE_PROJSPEED = 10.0f;
    void Start()
    {
        GameStateInformation.deployedCannons.Add(sampleCannon);
        GameStateInformation.deployedCannons.Add(sampleCannon2);
        GameStateInformation.deployedCannons.Add(sampleCannon3);
        gunnerRange = gunner.range;
        

    }

    // Update is called once per frame
    void Update()
    {
        selectedCannon = CannonDetected();
        Vector2 moveDirection;
        if (selectedCannon is not null)
        {
            //Debug.Log("selectedcannon is not null");
            
            if (LineOfSight(selectedCannon) != Vector2.zero && !InFiringRange())    //Line of sight established, gunner will move towards cannon
            {
                isFiring = false;
                if (firingCoroutine != null)
                {
                    StopCoroutine(firingCoroutine);
                    firingCoroutine = null;
                }
                moveDirection = LineOfSight(selectedCannon);    //calling LineOfSight() every update is probably reducing the performance
                GunnerMove(moveDirection);
            }

            else if (LineOfSight(selectedCannon) != Vector2.zero && InFiringRange() && !isFiring)
            {
                isFiring = true;
                moveDirection = LineOfSight(selectedCannon);
                if (firingCoroutine == null)
                {
                    firingCoroutine = StartCoroutine(BulletFire(moveDirection, selectedCannon));
                    Debug.Log(firingCoroutine);
                    Debug.Log("firing coroutine was null here");
                }
                Debug.Log("In firing range");
            }

            else if (LineOfSight(selectedCannon) == Vector2.zero)
            {
                Debug.Log("either there is no LOS or LOS function is not working properly");
            }
        }
        else
        {
            //Debug.Log("selected cannon is null. cannon not detetcted");
        }
    }


    void GunnerMove(Vector2 direction)
    {
        transform.position += new Vector3(direction.x * gunner.speed, direction.y * gunner.speed, 0);
    }

    Vector2 LineOfSight(GameObject selectedCannon)
    {
        if (selectedCannon == null)
        {
            return Vector2.zero;
        }
        Vector2 currentCoords = new Vector2(transform.position.x, transform.position.y);
        Vector2 currentCannonCoords = new Vector2(selectedCannon.transform.position.x, selectedCannon.transform.position.y);
        Vector2 direction = (selectedCannon.transform.position - transform.position).normalized;    //returns unit vector giving direction
        Vector2 collisionChecker = currentCoords;
        float rotationAngle = Mathf.Atan2(gunnerRange, Vector2.Distance(currentCannonCoords, collisionChecker));
        //Debug.Log("rotation angle: " + (rotationAngle * Mathf.Rad2Deg));

        while (true)
        {
            Vector3Int gridPoint = fortTiles.WorldToCell(new Vector3(collisionChecker.x, collisionChecker.y, 0) - new Vector3Int(1, 0, 0));     //point seems to point to the bottom right of the cell, (1, 0, 0) is subtracted to make it point to the bottom left.
            float checkerCannonDistance = Vector2.Distance(currentCannonCoords, collisionChecker);
            if (fortTiles.HasTile(gridPoint))
            {
                break;  //direct LOS to cannon is not possible, we will now check for tangents (in the next while loop)
            }
            if (checkerCannonDistance <= 2)
            {
                return direction;
            }
            collisionChecker += new Vector2(direction.x * test_increment_for_line_of_sight * Time.deltaTime, direction.y * test_increment_for_line_of_sight * Time.deltaTime);

        }

        collisionChecker = currentCoords;
        Vector2 direction_tangent1 = MathHelperFunctions.rotate(direction, rotationAngle);
        while (true)
        {
            Vector3Int gridPoint = fortTiles.WorldToCell(new Vector3(collisionChecker.x, collisionChecker.y, 0) - new Vector3Int(1, 0, 0));     //point seems to point to the bottom right of the cell, (1, 0, 0) is subtracted to make it point to the bottom left.
            float checkerCannonDistance = Vector2.Distance(currentCannonCoords, collisionChecker);
            if (fortTiles.HasTile(gridPoint))
            {
                break;  //LOS using first tangent to cannon is not possible, we will now check for second tangent (in the next while loop)
            }
            if (checkerCannonDistance <= 3)
            {
                return direction_tangent1;
            }
            collisionChecker += new Vector2(direction_tangent1.x * test_increment_for_line_of_sight * Time.deltaTime, direction_tangent1.y * test_increment_for_line_of_sight * Time.deltaTime);
        }

        collisionChecker = currentCoords;
        Vector2 direction_tangent2 = MathHelperFunctions.rotate(direction, -rotationAngle);
        while (true)
        {
            Vector3Int gridPoint = fortTiles.WorldToCell(new Vector3(collisionChecker.x, collisionChecker.y, 0) - new Vector3Int(1, 0, 0));     //point seems to point to the bottom right of the cell, (1, 0, 0) is subtracted to make it point to the bottom left.
            float checkerCannonDistance = Vector2.Distance(currentCannonCoords, collisionChecker);
            if (fortTiles.HasTile(gridPoint))
            {
                return Vector2.zero;  //LOS using second tangent to cannon is not possible
            }
            if (checkerCannonDistance <= 3)
            {
                return direction_tangent2;
            }
            collisionChecker += new Vector2(direction_tangent2.x * test_increment_for_line_of_sight * Time.deltaTime, direction_tangent2.y * test_increment_for_line_of_sight * Time.deltaTime);
        }

    }


    GameObject CannonDetected()
    {
        Vector2 currentCoords = new Vector2(transform.position.x, transform.position.y);
        float minDistance = float.MaxValue;
        if (GameStateInformation.deployedCannons.Count == 0 )
        {
            //Debug.Log("gamestateinformation says there are no cannons in the scene");
            return null;
        }
        GameObject nearestCannon = null;
        foreach (Transform cannonDeployed in GameStateInformation.deployedCannons)
        {
            Vector2 currentCannonCoords = new Vector2(cannonDeployed.transform.position.x, cannonDeployed.transform.position.y);
            float distance = Vector2.Distance(currentCoords, currentCannonCoords);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCannon = cannonDeployed.gameObject;
            }
        }

        //if (minDistance > cannonDetectRange)
        //{
          //  return null;
        //}

        return nearestCannon;

    }

    bool InFiringRange()
    {
        if (selectedCannon == null)
        {
            return false;
        }

        Vector2 currentCannonCoords = new Vector2(selectedCannon.transform.position.x, selectedCannon.transform.position.y);
        Vector2 currentCoords = new Vector2(transform.position.x, transform.position.y);
        float cannonGunnerDistance = Vector2.Distance(currentCoords, currentCannonCoords);
        if (cannonGunnerDistance > gunner.range)
        {
            return false;
        }

        else
        {
            return true;
        }
    }

    void GunnerFire(Vector3 direction)       //string selectedTargetType
    {
        //
        //Transform bullet = gameObject.transform.Find("Circle");
        //bullet.transform.position = Vector3.Lerp(bullet.transform.position, selectedCannon.transform.position, gunner.projectileSpeed * Time.deltaTime);
        //bullet.transform.position += new Vector3(direction.x * Time.deltaTime * gunner.projectileSpeed, direction.y * Time.deltaTime * gunner.projectileSpeed, 0);
        //bullet.transform.Translate(direction * Time.deltaTime * gunner.projectileSpeed);
    }

    IEnumerator BulletFire(Vector3 direction, GameObject targetCannon)    
    {
        while (isFiring)
        {
            if (targetCannon == null)
            {
                isFiring = false;
                yield break;
            }

            CannonStats cannonStats = targetCannon.GetComponent<CannonStats>();
            if (cannonStats.Health <= 0)
            {
                isFiring = false;
                yield break;
            }
            Transform bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            StartCoroutine(BulletTravel(direction, bullet, targetCannon));
            yield return new WaitForSeconds(SAMPLE_ROF);
        }
        
    }

    IEnumerator BulletTravel(Vector3 direction, Transform bullet, GameObject targetCannon)
    {
        while (bullet != null && targetCannon != null && Vector3.Distance(bullet.transform.position, selectedCannon.transform.position) > 0.1f)
        { 
            CannonStats cannonStats = targetCannon.GetComponent<CannonStats>();
            if (cannonStats.Health <= 0)
            {
                yield break;
            }
            bullet.transform.position = Vector3.Lerp(bullet.transform.position, selectedCannon.transform.position, SAMPLE_PROJSPEED * Time.deltaTime);
            yield return null;
        }
        Destroy(bullet.gameObject);
    }

}
