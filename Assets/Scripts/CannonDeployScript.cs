using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CannonDeployScript : MonoBehaviour
{
    public Transform cannonPrefab;
    public GameObject fortTilesGO;
    private Tilemap fortTiles;
    private Camera mainCam;
    void Start()
    {
        mainCam = Camera.main;
        fortTiles = fortTilesGO.GetComponent<Tilemap>();
    }

    void Update()
    {
        DeployCannon(0);
    }

    public bool CheckValidCannonPlacement()
    {

        Vector3Int deployGridPoint = fortTiles.WorldToCell(GetMouseWorldPosition());
        if (fortTiles.HasTile(deployGridPoint))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void DeployCannon(int directionmultiplier)
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && CheckValidCannonPlacement() && !CannonAlreadyPlaced())
        {
            Vector3Int tilemapPos = fortTiles.WorldToCell(GetMouseWorldPosition());
            Transform newCannon = Instantiate(cannonPrefab, fortTiles.GetCellCenterWorld(tilemapPos), Quaternion.identity);    //Add cannon facing directions

            GameStateInformation.deployedCannons.Add(newCannon);
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 point = mainCam.ScreenToWorldPoint(Input.mousePosition);
        point.z = 0f;
        return point;
    }

    public bool CannonAlreadyPlaced()
    {
        Vector3Int tilemapPos = fortTiles.WorldToCell(GetMouseWorldPosition());
        Vector3 center = fortTiles.GetCellCenterWorld(tilemapPos);
        foreach (Transform cannon in GameStateInformation.deployedCannons)
        {
            if (cannon.transform.position == center)
            {
                return true;
            }
        }
        return false;
    }

}
