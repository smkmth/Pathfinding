using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PathMover))]
public class Agent : MonoBehaviour
{
    //use this class to actually control the agent.
    private PathMover pathfinder;

    private void Start()
    {
        pathfinder = GetComponent<PathMover>();
    }

    // Update is called once per frame
    void Update()
    {
        //when we click, we get the clicked on worldpoint, convert it to 2dm then
        //call set destination on the point we find
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);
            pathfinder.SetDestination(worldPoint2d);

   
        }

    }
}
