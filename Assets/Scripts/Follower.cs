using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Movement {

    public Transform target;
    private Vector2 curPos;
    private Vector2 lastPos;
    private Pathfinder pathfinder;


	// Use this for initialization
	void Start () {
        lastPos = target.position;
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();


    }



    // Update is called once per frame
    void Update() {

        curPos = target.position;
        if (curPos == lastPos)
        {
            print("Not moving");
            lastPos = curPos;
            Node endnode = pathfinder.GetClosestNode(target.position);
            Node startnode = pathfinder.GetClosestNode(transform);
            SetPath(pathfinder.PathFind(startnode, endnode));

        }
        else
        {
            print("moving");
            lastPos = curPos;
        }
     	
	}
}
