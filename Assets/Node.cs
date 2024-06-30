using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 position;
    public bool walkable;
    public int gridX;
    public int gridY;

    public int gCost = 0;
    public int hCost = 0;
    public Node parent;

    public Node(Vector3 _position, bool _walkable, int _gridX, int _gridY)
    {
        position = _position;
        walkable = _walkable;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get { return gCost + fCost; }
    }
}
