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
    
    //get the closest node to a vector2 pos. 
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
    //get the cloest node to a given transform
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
    
    //get the distance used between two nodes. used in g cost.
    float GetDistance(Node node1, Node node2)
    {
        float DistX = Mathf.Abs(node1.location.position.x - node2.location.position.x);
        float DistY = Mathf.Abs(node1.location.position.y - node2.location.position.y);

        if (DistX > DistY)
            return 14 * DistY + 10 * (DistX - DistY);
        return 14 * DistX + 10 * (DistY - DistX);

    }
    
    public List<Node> PathFind(Vector2 start, Vector2 end)
    {
        //init variables i will need
        Node startnode = GetClosestNode(start);
        Node endnode = GetClosestNode(end);
        List<Node> openList = new List<Node>();     //open list for all the nodes the algorithim is considering
        List<Node> closedList = new List<Node>();   //closed list for the nodes that have already been delt with
        List<Node> path = new List<Node>();          //a list of nodes to form a path
        Node currentNode = new Node();               //the current node being investigated

        //start by adding the start point to our open lsit
        openList.Add(startnode);

        //While we still have options to consider
        while (openList.Count > 0)
        {
            currentNode = openList[0];

            //find the most promicing node to consider
            foreach (Node opennode in openList)
            {
                if (opennode.Fcost < currentNode.Fcost || opennode.Fcost == currentNode.Fcost && opennode.Hcost < currentNode.Hcost)
                {
                    currentNode = opennode;
                }
            }

            //we are now checking this node so take it from the open list and dump it in the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //if its the final node, we are done!
            if (currentNode == endnode)
            {
                return CalculatePath(startnode, endnode);
                
            }

            //if not, look at each of its neighbor nodes. 
            foreach (Node neighbor in currentNode.connections)
            {
                //if the neighbor is in the closed list we have already delt with it.
                if (closedList.Contains(neighbor))
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
    //here we calculate the actual path, by stepping backwards through 
    //the nodes, from the end node, and reading each previously value
    //inside each node. the end result is then reversed and returned.
    public List<Node> CalculatePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
      
        Node pathCurrentNode= end;
        while(pathCurrentNode != start)
        {

            path.Add(pathCurrentNode);
            pathCurrentNode = pathCurrentNode.previous;

        }
        
        path.Reverse();
            
        foreach(Node node in path)
        {
            node.ClearNode();
        }
        return path;
    }

    

}
