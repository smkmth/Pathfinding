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
    private List<Node> targetNodeList;
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
        currentMovestate = Movestate.TargetGiven;

        if (pathfinder.GetDistance(destination, transform.position) < fudgeDistance )
        {
            currentMovestate = Movestate.Finished;
            return;
        }

        Node destinationNode = pathfinder.CheapGetClosestNode(destination);
            
        if (destinationNode.walkable == false)
        {
            currentMovestate = Movestate.Error;
            return;
        }

        SetPath(pathfinder.PathFind(transform.position, destination));

    }

    //set path validates the path, to make sure it makes sence then sets it to 
    //targetNodeList and sets the movestate so we start moving.
    public void SetPath(List<Node> targetlist)
    {

        if (targetlist.Count <= 0)
        {
            currentMovestate = Movestate.Error;
        }
        else
        {
            int targetindex = 0;
            foreach (Node target in targetlist)
            {
                if (target == null) {
                    Debug.Assert(false, "Target " + targetindex + " is null for " + gameObject.name);
                    currentMovestate = Movestate.Error;
                }
                targetindex++;
            }

            targetNodeList = targetlist;     
        }
    }
	
	// Update is called once per frame
	void Update () {

  

        //if we have a path, we move our transfrom towards the taget at a speed. else, we either increment 
        //the targetListIndex to the next target node, or we are finished, and awaiting orders. 
        if (currentMovestate != Movestate.Error && currentMovestate != Movestate.Finished)
        {
            
            distance = Vector2.Distance(transform.position, targetNodeList[targetListIndex].location.position);
            if (distance > fudgeDistance)
            {
                float step = movespeed * Time.deltaTime;
                Vector3 mov = Vector3.MoveTowards(transform.position, targetNodeList[targetListIndex].location.position, step);
                transform.position = mov;
                currentMovestate = Movestate.Moving;

            }
            else
            {
                if (targetListIndex >= (targetNodeList.Count - 1))
                {

                    currentMovestate = Movestate.Finished;
                    targetListIndex = 0;
                }
                else
                {
                    targetListIndex += 1;
                }
            }
        }

        
		
	}
}
