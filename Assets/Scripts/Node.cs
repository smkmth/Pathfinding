using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//raycast node implementation. this class will scan 8 directions
//around it and store a list of nodes. 

//********************************************************************

//IMPORTANT NOTE. THIS WILL ONLY WORK ATM IF IN THE PHYSICS2D SETTINGS
//QUERIES START IN COLLIDERS IS UNCHECKED.

//********************************************************************

//SIMILARLY, NODES NEED TO BE ON A LAYER CALLED "Node" AND OBJECTS TO 
//AVOID NEED TO BE ON A LAYER CALLED "Object"

//********************************************************************
public class Node : MonoBehaviour {


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


    // Use this for initialization
    void Start()
    {

        location = transform;
        previous = this;
        
    }

    

    public void CalculateNeighbors()
    {

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

    } 



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
        previous = this;
        Hcost = 0;
        Gcost = 0;

    }
}
