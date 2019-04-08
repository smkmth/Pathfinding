using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path  {

    public readonly Vector2[] lookPoints;
    public readonly Line[] turnBounaries;
    public readonly int finishLineIndex;


    public Path( Vector2[] waypoints, Vector2 startpos, float turnDist)
    {
        lookPoints = waypoints;
        turnBounaries = new Line[lookPoints.Length];
        finishLineIndex = turnBounaries.Length - 1;
        Vector2 previousPoint = startpos;
        for (int i =0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = lookPoints[i];
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDist;
            turnBounaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDist);
            previousPoint = turnBoundaryPoint;
        }

    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach(Vector2 p in lookPoints)
        {
            Gizmos.DrawCube(p, Vector2.one );
        }

        Gizmos.color = Color.white;
        foreach(Line l in turnBounaries)
        {
            l.DrawWithGizmos(10);
        }
    }
}
