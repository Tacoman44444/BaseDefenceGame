using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask walls;
    public Vector2 gridWorldSize;
    public float nodeEdgeSize;
    private Node[,] grid;

    private int gridWorldSizeX, gridWorldSizeY;
    void Start()
    {
        gridWorldSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeEdgeSize);
        gridWorldSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeEdgeSize);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridWorldSizeX, gridWorldSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x - Vector3.up * gridWorldSize.y;

        for (int i = 0;  i < gridWorldSizeX; i++)
        {
            for (int j = 0; i < gridWorldSizeY; j++)
            {
                Vector3 worldPoint = bottomLeft + new Vector3(nodeEdgeSize * i, nodeEdgeSize * j, 0);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeEdgeSize / 2, walls));
                grid[i, j] = new Node(worldPoint, walkable, i, j);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPoint)
    {
        float percentX = (worldPoint.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPoint.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridWorldSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridWorldSizeY - 1) * percentY);
        return grid[x, y];
    }

    public int[] GetGridPosition(Vector3 nodePosition)
    {
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x - Vector3.up * gridWorldSize.y;
        Vector3 pp = nodePosition - bottomLeft;
        int[] pos = new int[2] { Convert.ToInt32(pp.x / nodeEdgeSize), Convert.ToInt32(pp.y / nodeEdgeSize) };
        return pos;
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1;  y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridWorldSizeX && checkY >= 0 && checkY < gridWorldSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

}
