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

public class Movement : MonoBehaviour {

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

    //set path validates the path, to make sure it makes sence then sets it to 
    //targetNodeList and sets the movestate so we start moving.
    public void SetPath(List<Node> targetlist)
    {
        currentMovestate = Movestate.TargetGiven;
        if (targetlist.Count <= 0)
        {
            currentMovestate = Movestate.Error;
            Debug.Assert(false, "No targets passed to " + gameObject.name);

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

        //when we click, we get the clicked on worldpoint, convert it to 2dm then set a path
        //that we make from pathfinder, from the current pos, to the mouse click pos.
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);
            SetPath(pathfinder.PathFind(transform.position, worldPoint2d));

        }

        //if we have a path, we move our transfrom towards the taget at a speed. else, we either increment 
        //the targetListIndex to the next target node, or we are finished, and awaiting orders. 
        if (currentMovestate != Movestate.Error)
        {

            distance = Vector3.Distance(transform.position, targetNodeList[targetListIndex].location.position);
            if (distance > fudgeDistance)
            {
                float step = movespeed * Time.deltaTime;
                Vector3 mov = Vector3.MoveTowards(transform.position, targetNodeList[targetListIndex].location.position, step);
                transform.position = mov;
                currentMovestate = Movestate.Moving;

            }
            else
            {
                currentMovestate = Movestate.Finished;
                if (targetListIndex >= (targetNodeList.Count - 1))
                {
                    currentMovestate = Movestate.Error;
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
