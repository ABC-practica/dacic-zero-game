using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;

    Grid Grid;
    void Awake()
    {
        Grid = GetComponent<Grid>();
    }

    void Update()
    {
            FindPath(seeker.position, target.position);
        
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        

        Node startNode = Grid.NodeFromWorldPoint(startPos);
        Node targetNode = Grid.NodeFromWorldPoint(targetPos);
        Heap<Node> openSet = new Heap<Node>(Grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);
            if (currentNode == targetNode)
            {
                
                //print("Path found: ");
                RetracePath(startNode, targetNode);
                return;
            }
            foreach (Node neighbour in Grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
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
        Grid.path = path;
    }
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

}
    