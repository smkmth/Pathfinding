//uncomment out to see path, start point and destination
#define DEBUGDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The class that actually does the pathfinding implementation.
//to work, this class needs some kind of node class, which has a 
//value for gcost, hcost and fcost, as well as an empty reference 
//to another node, and a list of connections to adjacent nodes.

public class Pathfinder : MonoBehaviour {

    //A list of nodes for the pathfinder to navigate
    public List<Node> Nodes;
    PathRequestManager requestManager;


    public void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector2 start, Vector2 end)
    {
        StartCoroutine(PathFind(start, end));
    }

    public IEnumerator PathFind(Vector2 start, Vector2 end)
    {
        bool pathSucess = false;
        Vector2[] waypoints = new Vector2[0];

        foreach (Node node in Nodes)
        {
            node.ClearNode();
        }
        //init variables i will need
        Node startnode = CheapGetClosestNode(start);
        Node endnode = CheapGetClosestNode(end);
        if (startnode.walkable && endnode.walkable)
        {

            Heap<Node> openList = new Heap<Node>(Nodes.Count);     //open list for all the nodes the algorithim is considering

            List<Node> path = new List<Node>();          //a list of nodes to form a path
            Node currentNode = startnode;           //the current node being investigated

            //start by adding the start point to our open lsit
            openList.Add(startnode);

            //While we still have options to consider
            while (openList.Count > 0)
            {

                //the heap finds the next most promicing item in the open list
                currentNode = openList.RemoveFirst();
                //we are now checking this node so take it from the open list and dump it in the closed list
                currentNode.inClosedList = true;

                //if its the final node, we are done!
                if (currentNode == endnode)
                {
                    pathSucess = true;
                    break;


                }

                //if not, look at each of its neighbor nodes. 
                foreach (Node neighbor in currentNode.connections)
                {
#if DEBUGDRAW
                    Debug.DrawLine(currentNode.transform.position, neighbor.transform.position, Color.red, 3.0f);
#endif
                    //if the neighbor is in the closed list we have already delt with it.
                    if (neighbor.inClosedList == true || neighbor.walkable == false)
                    {
                        continue;
                    }

                    //else calculate a potential move cost, by adding the distance to this currentnode and its neigbour to the
                    //current gcost. 
                    float newMovementCost = currentNode.Gcost + GetDistance(currentNode, neighbor);

                    //if it is better then the neighbor gcost then use it. if it is not in the open list, add it to the open list
                    if (newMovementCost < neighbor.Gcost || !openList.Contains(neighbor))
                    {
                        //we now set the g cost of the neighbor, which is how quickly i can get to the neigbor node
                        neighbor.Gcost = newMovementCost;
                        //we then set the h cost, which is how close the neighbor is to the end.
                        //the combined f cost is our heuristic for evaluating which node to check next.
                        neighbor.Hcost = GetDistance(neighbor, endnode);

                        //lastly we set the neighbors previous value, which is a reference to the previous point on the path
                        //really important as that is how we will ultimately find the path in calculate path.
                        neighbor.previous = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }

                }
            }
            //if we get here, the node is impossible to reach! we simply reset the nodes and return a null path. 
            if (openList.Count == 0)
            {

                //Debug.LogWarning("Path not solved. Are you checking if the destination is walkable?");

                //   openList.Clear();
            }
        }

        yield return null;
        if (pathSucess)
        {
            waypoints = CalculatePath(startnode, endnode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSucess);


    }
    //here we calculate the actual path, by stepping backwards through 
    //the nodes, from the end node, and reading each previously value
    //inside each node. the end result is then reversed and returned.
    public Vector2[] CalculatePath(Node start, Node end)
    {
        
        List<Node> path = new List<Node>();

        Node pathCurrentNode= end;
        while(pathCurrentNode != start)
        {

#if DEBUGDRAW
            Debug.DrawLine(pathCurrentNode.transform.position, pathCurrentNode.previous.transform.position, Color.yellow, 4.0f);
#endif
            path.Add(pathCurrentNode);
            pathCurrentNode = pathCurrentNode.previous;

        }
        
        Vector2[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].transform.position.x - path[i].transform.position.x, path[i - 1].transform.position.y- path[i].transform.position.y);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].transform.position);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    //NODE FINDING!

    //get the closest node to a vector2 pos. Obsolete, use CheapGetClosestNode
    public Node GetClosestNode(Vector2 pos)
    {
        Node currentclosest = Nodes[0];

        float bestdistance = 9999999.0f;
        foreach (Node node in Nodes)
        {
            float currentdistance = Vector2.Distance(pos, node.location.position);
            if (currentdistance < bestdistance)
            {
                bestdistance = currentdistance;
                currentclosest = node;
            }
        }
        // Debug.LogWarning("Caution; using the expensive get closest node. consider using CheapGetClosestNode");
        return currentclosest;
    }
    //cheap get closest node returns in 0.045ms, verces get closest which takes about 18ms, uses 
    //manhatten distance instead of squrt which is major league cheaper
    public Node CheapGetClosestNode(Vector2 pos)
    {
        Node currentclosest = Nodes[0];
        float bestdistance = 9999999.0f;
        Collider2D[] results = Physics2D.OverlapCircleAll(pos, 3.0f, LayerMask.GetMask("Node"));
        foreach (Collider2D result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("Node"))
            {
                Node currentClosest = result.GetComponent<Node>();
                float testdistance = GetDistance(currentClosest, pos);
                if (bestdistance > testdistance)
                {
                    bestdistance = testdistance;
                    currentclosest = currentClosest;
                }

            }
        }
#if DEBUGDRAW
        currentclosest.transform.GetChild(0).gameObject.SetActive(true);
        currentclosest.GetComponentInChildren<SpriteRenderer>().color = Color.red;
#endif

        return currentclosest;
    }

    //get the cloest node to a given transform
    public Node GetClosestNode(Transform pos)
    {
        Node currentclosest = new Node();

        float bestdistance = 9999999.0f;
        foreach (Node node in Nodes)
        {
            float currentdistance = Vector2.Distance(pos.position, node.location.position);
            if (currentdistance < bestdistance)
            {
                bestdistance = currentdistance;
                currentclosest = node;
            }
        }
        return currentclosest;
    }

    //get the distance used between two nodes. used in g cost.
    public float GetDistance(Node node1, Node node2)
    {
        float DistX = Mathf.Abs(node1.location.position.x - node2.location.position.x);
        float DistY = Mathf.Abs(node1.location.position.y - node2.location.position.y);

        if (DistX > DistY)
            return 14 * DistY + 10 * (DistX - DistY);
        return 14 * DistX + 10 * (DistY - DistX);
    }

    //get the distance used between a node and a pos.
    public float GetDistance(Node node1, Vector2 pos)
    {
        float DistX = Mathf.Abs(node1.location.position.x - pos.x);
        float DistY = Mathf.Abs(node1.location.position.y - pos.y);

        if (DistX > DistY)
            return 14 * DistY + 10 * (DistX - DistY);
        return 14 * DistX + 10 * (DistY - DistX);

    }

    //get the distance used between a pos and a pos.
    public float GetDistance(Vector2 pos1, Vector2 pos2)
    {
        float DistX = Mathf.Abs(pos1.x - pos2.x);
        float DistY = Mathf.Abs(pos1.y - pos2.y);

        if (DistX > DistY)
            return 14 * DistY + 10 * (DistX - DistY);
        return 14 * DistX + 10 * (DistY - DistX);

    }




}
