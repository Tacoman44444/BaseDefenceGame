using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class DamageScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("gunner"))
        {
            //Debug.Log("WORKS"); //
            //Debug.Log(GameStateInformation.damagedWalls.Count); //
            Tilemap tilemap = GetComponent<Tilemap>();
            Vector3 contactPoint = collision.GetContact(0).point;
            //Debug.Log(contactPoint);
            Vector3Int gridPoint = tilemap.WorldToCell(contactPoint) - new Vector3Int(1, 0, 0); //gridpoint seems to point to the bottom right of the cell, (1, 0, 0) is subtracted to make it point to the bottom left.
            //Debug.Log(gridPoint);

            if (tilemap.HasTile(gridPoint))
            {
                TileBase collidedTile = tilemap.GetTile(gridPoint);
                if (!GameStateInformation.damagedWalls.Contains(collidedTile))
                {
                    GameStateInformation.damagedWalls.Add(collidedTile, 10);
                }
                else
                {
                    int currentDamage = (int)GameStateInformation.damagedWalls[collidedTile];
                    GameStateInformation.damagedWalls[collidedTile] = currentDamage + 10;
                }
            }
        }
    }
}
