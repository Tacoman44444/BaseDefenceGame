using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditorInternal;
using UnityEngine;

public class AstarSearchAI : MonoBehaviour
{
    // Start is called before the first frame update
    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void PathToTower(Transform fighter, GameObject detectedCannon)     //returning the node
    {
        Vector3 fighterLocation = fighter.position;
        Vector3 CannonLocation = detectedCannon.transform.position;
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        Node startNode = grid.NodeFromWorldPoint(fighterLocation);
        Node targetNode = grid.NodeFromWorldPoint(CannonLocation);
        open.Add(startNode);

        while (open.Count > 0)
        {
            Node current = LeastCost(open);
            open.Remove(current);
            closed.Add(current);

            if (current == targetNode)
            {
                return;
            }

            foreach (Node neighbour in grid.GetNeighbors(current))
            {
                if (!neighbour.walkable || closed.Contains(neighbour))
                {
                    continue;
                }

                int newNeighborMvmtCost = current.gCost + GetDistance(current, neighbour);
                if (newNeighborMvmtCost > neighbour.gCost || !open.Contains(neighbour))
                {
                    neighbour.gCost = newNeighborMvmtCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = current;

                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }

            }
              
        }
    } 

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
    }

    int GetDistance (Node NodeA, Node NodeB)
    {
        int dstX = Mathf.Abs(NodeA.gridX - NodeB.gridX);
        int dstY = Mathf.Abs(NodeA.gridY - NodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY - 10 * (dstX - dstY);
        return 14*dstX - 10 * (dstY - dstX);
    }

    Node LeastCost(List<Node> openList)
    {
        Node minNode = openList[0];
        foreach (Node node in openList)
        {
            if (node.fCost < minNode.fCost || node.fCost == minNode.fCost && node.hCost < minNode.hCost)
            {
                minNode = node;
            }
        }
        return minNode;
    }
}
