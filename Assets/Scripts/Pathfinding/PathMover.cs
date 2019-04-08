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

public enum FacingDirection
{
    UP = 270,
    DOWN = 90,
    LEFT = 180,
    RIGHT = 0
}

public class PathMover : MonoBehaviour {

    //what state this ai currently is
    public Movestate currentMovestate;

    //our movespeed, and how close we need to be
    //to a target to consider the path finished.
    public float movespeed;
    public float turnSpeed = 3;
    public float fudgeDistance;

    //private vars that store the list of targets, the index
    //for the target we are currently going for and the 
    //distance from current target.
    //public Vector2[] path;
    private int targetListIndex;
    public float turnDistance;
    private float distance;
    const float pathUpdateMoveThreshold = .5f;

    Path path;
    Vector2 target;
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
        target = destination;

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
            path = new Path(newPath, transform.position, turnDistance);
            Debug.Log("PathFound!");
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
           // targetListIndex = 0;

        }
        else
        {
            currentMovestate = Movestate.Error;
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);
        while (followingPath)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            while (path.turnBounaries[pathIndex].HasCrossedLine(pos))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;

                }
            }
            if (followingPath)
            {
                transform.rotation = FaceObject(pos, path.lookPoints[pathIndex], FacingDirection.DOWN);
                transform.Translate(Vector3.up * Time.deltaTime * movespeed, Space.Self);
            }
            yield return null;
        }
    }

    public static Quaternion FaceObject(Vector2 startingPosition, Vector2 targetPosition, FacingDirection facing)
    {
        Vector2 direction = targetPosition - startingPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= (float)facing;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }


    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
