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

    public Movestate currentMovestate;
    private Pathfinder pathfinder;
    public List<Transform> targetList;
    public List<Node> targetNodeList;
    public int targetListIndex;
    public float movespeed;
    public float distance;
    public float fudgeDistance;

	// Use this for initialization
	void Start () {
        targetListIndex = 0;

        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
      
		
	}

    public void SetPath(List<Transform> targetlist)
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
            foreach (Transform target in targetlist)
            {
                if (target == null) {
                    Debug.Assert(false, "Target " + targetindex + " is null for " + gameObject.name);
                    currentMovestate = Movestate.Error;
                }
                targetindex++;
            }
            targetList = targetlist;     
        }
    }
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


        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MouseDown");
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);
            // Transform mousepos = transform;
            //mousepos.position = worldPoint;
            Node endnode = pathfinder.GetClosestNode(worldPoint2d);
            Node startnode = pathfinder.GetClosestNode(transform);
            Debug.Log("Start node was " + startnode.name);
            Debug.Log("end node was " + endnode.name);
            SetPath(pathfinder.PathFind(startnode, endnode));

        }


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
