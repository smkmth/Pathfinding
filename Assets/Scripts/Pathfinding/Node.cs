using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//primary implementation is physics overlap checking. will work in 
// most situations. objects are marked as walkable by obstructions,
//so anything which obstructs a node must have an obstruction script
//
//This does however work dynamically, meaning nodes can be obscured 
//at run time with the obstruction object and still work.

//raycast node implementation. this class will scan 8 directions
//around it and store a list of nodes. 

//IMPORTANT NOTE. THIS WILL ONLY WORK ATM IF IN THE PHYSICS2D SETTINGS
//QUERIES START IN COLLIDERS IS UNCHECKED.

//********************************************************************

//FOR BOTH IMPLEMENTATIONS, NODES NEED TO BE ON A LAYER CALLED "Node" 
//AND OBJECTS TO AVOID NEED TO BE ON A LAYER CALLED "Object"

//********************************************************************
public class Node : MonoBehaviour, IHeapItem<Node>{


    [HideInInspector]
    public float DistanceToCheck;
    [HideInInspector]
    public float RadiusToCheck;

    public List<Node> connections;
    public Transform location;

    private Vector2 downleft;
    private Vector2 downright;
    private Vector2 forwardleft;
    private Vector2 forwardright;

    public LayerMask NodesToCheck;
    public LayerMask WallsToCheck;


    //pathfinding stuff
    public Node previous;
    public float Gcost;
    public float Hcost;
    public float Fcost
    {
        get
        {
            return Gcost + Hcost;
        }
    }

    //heap stuff
    int heapIndex;

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public bool walkable;
    public bool inClosedList;


    // Use this for initialization
    void Awake()
    {
        walkable = true;
        inClosedList = false;
        location = transform;
        previous = this;
        
    }

    

    public void CalculateNeighbors()
    {
        /*

        forwardright = (Vector2.up + Vector2.right).normalized;
        forwardleft = (Vector2.up + Vector2.left).normalized;

        downright = (Vector2.down + Vector2.right).normalized;
        downleft = (Vector2.down + Vector2.left).normalized;

        CheckDirection(Vector2.right, DistanceToCheck, RadiusToCheck);
        CheckDirection(Vector2.left, DistanceToCheck, RadiusToCheck);
        CheckDirection(Vector2.up, DistanceToCheck, RadiusToCheck);
        CheckDirection(Vector2.down, DistanceToCheck, RadiusToCheck);
        
        CheckDirection(forwardleft, DistanceToCheck, RadiusToCheck);
        CheckDirection(forwardright, DistanceToCheck, RadiusToCheck);
        CheckDirection(downleft, DistanceToCheck, RadiusToCheck);
        CheckDirection(downright, DistanceToCheck, RadiusToCheck);
        */
        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, RadiusToCheck, NodesToCheck );
        foreach(Collider2D result in results)
        {
            if(result.gameObject.layer == LayerMask.NameToLayer("Node"))
            {
                connections.Add(result.gameObject.GetComponent<Node>());
            }
        }
         

     

    } 


    //raycast node code is not currently used, node walkable is set by the objects themselves
    void CheckDirection( Vector2 directionToCheck, float distanceToCheck, float radiustocheck)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radiustocheck , directionToCheck , distanceToCheck);
        

        // Does the ray intersect any objects?
        if (hit.collider != null)
        {
            //for debug purposes so we can see any collisions
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Object"))
            {
                Debug.DrawLine(transform.position, directionToCheck * hit.distance, Color.yellow,30.0f);
                //Debug.Log(gameObject.name + "Did Hit " + hit.transform.gameObject.name + " to the " + directionToCheck);
                return;
            }


            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Node"))
            {
                Node node = hit.collider.gameObject.GetComponent<Node>();

                connections.Add(hit.collider.gameObject.GetComponent<Node>());

                Debug.DrawLine(transform.position, directionToCheck * hit.distance, Color.blue,5.0f);
                //Debug.Log(gameObject.name + "can travel to " + hit.collider.gameObject.name);
              
            }
        }
        else
        {
            Debug.DrawRay(transform.position,directionToCheck * distanceToCheck, Color.white, 30.0f);
        }


    }

    public void ClearNode()
    {
        inClosedList = false;
        previous = this;
        Hcost = 0;
        Gcost = 0;

    }

    public int CompareTo(Node other)
    {
        int compare = Fcost.CompareTo(other.Fcost);
        if (compare == 0){
            compare = Hcost.CompareTo(other.Hcost);
        }
        return -compare;

    }
}
