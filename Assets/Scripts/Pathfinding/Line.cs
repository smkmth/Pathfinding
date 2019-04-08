using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line  {

    const float verticlaLineGradient = 1e5f;

    float gradient;
    float y_intercept;

    float gradientPerpendicular;

    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;
    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;
        if (dx == 0)
        {
            gradientPerpendicular = verticlaLineGradient;
        }
        else
        {
            gradientPerpendicular = dy / dx;

        }
        if (gradientPerpendicular == 0)
        {
            gradient = verticlaLineGradient;
        }
        else
        {
            gradient = -1 / gradientPerpendicular;
        }

        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradient);
        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    bool GetSide(Vector2 p)
    {
        return ((p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x));
        
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;

    }

    public void DrawWithGizmos(float length)
    {
        Vector2 lineDirection = new Vector2(1, gradient).normalized;
        Vector2 lineCenter = new Vector2(pointOnLine_1.x, pointOnLine_1.y);

        Gizmos.DrawLine(lineCenter - lineDirection * length / 2f, lineCenter + lineDirection * length / 2f);
    }

}
