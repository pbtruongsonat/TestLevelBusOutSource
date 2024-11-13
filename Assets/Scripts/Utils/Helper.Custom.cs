using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class Helper
{
    public static List<Vector2> GetFrontPointsCollider(this PolygonCollider2D polygonCollider2D, Direction direction)
    {
        var ans = new List<Vector2>();

        var points = polygonCollider2D.points;

        if (direction == Direction.Right)
        {
            direction = Direction.Left;
        }
        else if (direction == Direction.TopRight)
        {
            direction = Direction.TopLeft;
        }
        else if (direction == Direction.DownRight)
        {
            direction = Direction.DownLeft;
        }

        switch (direction)
        {
            case Direction.Top:
                ans = points.OrderByDescending(p => p.y).Take(2).ToList();
                break;

            case Direction.Down:
                ans = points.OrderBy(p => p.y).Take(2).ToList();
                break;

            case Direction.DownLeft:
                ans = points.OrderBy(p => p.x).ThenBy(p => p.y).Take(2).ToList();
                break;

            case Direction.Left:
                ans = points.OrderBy(p => p.x).Take(2).ToList();
                break;

            case Direction.TopLeft:
                ans = points.OrderBy(p => p.x).ThenByDescending(p => p.y).Take(2).ToList();
                break;
        }

        return ans;
    }


}
