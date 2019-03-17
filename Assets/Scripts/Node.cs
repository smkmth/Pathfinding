using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {



    public float DistanceToCheck =1;
    public float RadiusToCheck =1;

    

    public List<Node> connections;
    public Transform location;

    public Vector2 downleft;
    public Vector2 downright;
    public Vector2 forwardleft;
    public Vector2 forwardright;

    public Node previous;
    public float g;
    public float h;
    public float f
    {
        get
        {
            return g + h;
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
        // Debug.DrawRay(transform.position, directionToCheck * distanceToCheck, Color.white, 30.0f);
        //RaycastHit2D hit = Physics2D.Raycast(transform.position , directionToCheck );
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radiustocheck , directionToCheck , distanceToCheck);
        // Does the ray intersect any objects excluding the player layer
        if (hit.collider != null)
        {
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
        h = 0;
        g = 0;

    }
}
