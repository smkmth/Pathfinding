using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    public List<Transform> Destinations;

    public List<Node> Nodes;

    public List<Movement> Agents;

    public Transform Destination;

    public List<Transform> agent1Dest;
    public List<Transform> agent2Dest;
    public List<Node> path;

    public List<Node> openList;
    public List<Node> closedList;

    public bool searching;
    public Node currentNode;
    public bool done;
    public int costsofar;
    public bool pathfinding;


    public Node GetClosestNode(Vector2 pos)
    {
        float bestdistance = 9999999.0f;
        Node currentclosest = new Node();
        foreach( Node node in Nodes)
        {
            float currentdistance = Vector2.Distance(pos, node.transform.position);
            if (currentdistance < bestdistance)
            {
                bestdistance = currentdistance;
                currentclosest = node;
            }
        }
        return currentclosest;
    }

    public Node GetClosestNode(Transform pos)
    {
        float bestdistance = 9999999.0f;
        Node currentclosest = new Node();
        foreach (Node node in Nodes)
        {
            float currentdistance = Vector2.Distance(pos.position, node.transform.position);
            if (currentdistance < bestdistance)
            {
                bestdistance = currentdistance;
                currentclosest = node;
            }
        }

        return currentclosest;
    }


    private void Start()
    {
        pathfinding = false;
    }

   

    float GetDistance(Node node1, Node node2)
    {
        float DistX = Mathf.Abs(node1.location.position.x - node2.location.position.x);
        float DistY = Mathf.Abs(node1.location.position.y - node2.location.position.y);

        if (DistX > DistY)
            return 14 * DistY + 10 * (DistX - DistY);
        return 14 * DistX + 10 * (DistY - DistX);

    }
    
    public List<Node> PathFind(Node start, Node end)
    {

        path.Clear();
        openList.Add(start);
        while (openList.Count > 0)
        {
            currentNode = openList[0];
            foreach (Node opennode in openList)
            {
                if (opennode.f < currentNode.f || opennode.f == currentNode.f && opennode.h < currentNode.h)
                {
                    currentNode = opennode;
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);


            if (currentNode == end)
            {
                return CalculatePath(start, end);
                
            }
            foreach (Node neighbor in currentNode.connections)
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                float newMovementCost = currentNode.g + GetDistance(currentNode, neighbor);
                if (newMovementCost < neighbor.g || !openList.Contains(neighbor))
                {
                    neighbor.g = newMovementCost;
                    neighbor.h = GetDistance(neighbor, end);
                    neighbor.previous = currentNode;
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }

            }
        }
        if (openList.Count == 0)
        {
            Debug.Log("Cant solve");
            closedList.Clear();
            openList.Clear();

            foreach (Node node in Nodes)
            {
                node.ClearNode();
            }

        }
        return path;


    }

    public List<Node> CalculatePath(Node start, Node end)
    {
      
        Node pathCurrentNode= end;
        while(pathCurrentNode != start)
        {

            path.Add(pathCurrentNode);
            pathCurrentNode = pathCurrentNode.previous;

        }
        
        path.Reverse();


        closedList.Clear();
        openList.Clear();
            
        foreach(Node node in path)
        {
            node.ClearNode();
        }
        return path;
    }

    

}
