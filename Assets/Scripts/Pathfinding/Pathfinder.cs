//uncomment out to see path, start point and destination
#define DEBUGDRAW
//#define USELIST
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


    public List<Node> PathFind(Vector2 start, Vector2 end)
    {
        foreach (Node node in Nodes)
        {
            node.ClearNode();
        }
        //init variables i will need
        Node startnode = CheapGetClosestNode(start);
        Node endnode = CheapGetClosestNode(end);
#if USELIST
        List<Node> openList = new List<Node>();     //open list for all the nodes the algorithim is considering
#else
        Heap<Node> openList = new Heap<Node>(Nodes.Count);     //open list for all the nodes the algorithim is considering
#endif
        List<Node> path = new List<Node>();          //a list of nodes to form a path
        Node currentNode = startnode;           //the current node being investigated

        //start by adding the start point to our open lsit
        openList.Add(startnode);

        //While we still have options to consider
        while (openList.Count > 0)
        {

#if USELIST
            currentNode = openList[0];
#else
            currentNode = openList.RemoveFirst();
#endif


#if USELIST
            //find the most promicing node to consider
            foreach (Node opennode in openList)
            {
                if (opennode.Fcost < currentNode.Fcost || opennode.Fcost == currentNode.Fcost && opennode.Hcost < currentNode.Hcost)
                {
                    currentNode = opennode;
                }
            }
#endif



            //we are now checking this node so take it from the open list and dump it in the closed list
#if USELIST
            openList.Remove(currentNode);
#endif
            currentNode.inClosedList = true;

            //if its the final node, we are done!
            if (currentNode == endnode)
            {
                //sw.Stop();
                //print("PathFound : " + sw.ElapsedMilliseconds + "ms");
                return CalculatePath(startnode, endnode);
                
            }

            //if not, look at each of its neighbor nodes. 
            foreach (Node neighbor in currentNode.connections)
            {
#if DEBUGDRAW
                Debug.DrawLine(currentNode.transform.position, neighbor.transform.position,Color.red, 3.0f);
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
        return path;


    }
    //here we calculate the actual path, by stepping backwards through 
    //the nodes, from the end node, and reading each previously value
    //inside each node. the end result is then reversed and returned.
    public List<Node> CalculatePath(Node start, Node end)
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
        
        path.Reverse();
        return path;
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
