using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Movestate
{
    Error,
    TargetGiven,
    Moving,
    Finished
}

public class PathMover : MonoBehaviour {

    //what state this ai currently is
    public Movestate currentMovestate;

    //our movespeed, and how close we need to be
    //to a target to consider the path finished.
    public float movespeed;
    public float fudgeDistance;

    //private vars that store the list of targets, the index
    //for the target we are currently going for and the 
    //distance from current target.
    public Vector2[] path;
    private int targetListIndex;
    private float distance;
    
    //a ref to our pathfinder 
    private Pathfinder pathfinder;
	
    // Use this for initialization
	void Start ()
    {
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        
        targetListIndex = 0;
	}


    //Takes a vec2, checks if it a valid point, then calls set path of that destination
    public void SetDestination(Vector2 destination)
    {
        targetListIndex = 0;

        if (pathfinder.GetDistance(destination, transform.position) < fudgeDistance )
        {
            currentMovestate = Movestate.Finished;
            return;
        }
        PathRequestManager.RequestPath(transform.position, destination, OnPathFound);

    }
    public void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {

        if (pathSuccessful)
        {
            currentMovestate = Movestate.TargetGiven;
            path = newPath;
            Debug.Log("PathFound!");
            targetListIndex = 0;

        }
        else
        {
            currentMovestate = Movestate.Error;
        }
    }


 
	
	// Update is called once per frame
	void Update () {

        //if we have a path, we move our transfrom towards the taget at a speed. else, we either increment 
        //the targetListIndex to the next target node, or we are finished, and awaiting orders. 
        if (currentMovestate != Movestate.Error && currentMovestate != Movestate.Finished)
        {
            if (targetListIndex >= (path.Length))
            {
                currentMovestate = Movestate.Finished;
                Debug.Log("Finished!");

                targetListIndex = 0;
                return;
                
            }
            
            distance = Vector2.Distance(transform.position, path[targetListIndex]);
            if (distance > fudgeDistance)
            {
                float step = movespeed * Time.deltaTime;
                Vector3 mov = Vector3.MoveTowards(transform.position, path[targetListIndex], step);
                transform.position = mov;
                currentMovestate = Movestate.Moving;

            }
            else
            {
                targetListIndex += 1;

            }

        }

        
		
	}

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetListIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetListIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
