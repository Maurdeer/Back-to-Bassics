using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    None = 0,
    North,
    South,
    East,
    West,
    Northeast,
    Northwest,
    Southeast,
    Southwest
}
public static class DirectionHelper
{
    const float inbetween = 22.5f;
    public static Direction GetVectorDirection(Vector2 line)
    {
        if (line == Vector2.zero)
        {
            return Direction.None;
        }

        float angle = Vector2.SignedAngle(line.normalized, Vector2.up);

        if (angle >= 0)
        {
            if (angle >= 0 && angle < 45)
            {
                return angle >= inbetween ? Direction.Northeast : Direction.North;
            }
            else if (angle >= 45 && angle < 90)
            {
                return angle - 45 >= inbetween ? Direction.East : Direction.Northeast;
            }
            else if (angle >= 90 && angle < 135)
            {
                return angle - 90 >= inbetween ? Direction.Southeast : Direction.East;
            }
            else // angle >= 135
            {
                return angle - 135 >= inbetween ? Direction.South : Direction.Southeast;
            }
        }

        // So that we can work with a positive number
        angle = -angle;
        if (angle >= 0 && angle < 45)
        {
            return angle >= inbetween ? Direction.Northwest : Direction.North;
        }
        else if (angle >= 45 && angle < 90)
        {
            return angle - 45 >= inbetween ? Direction.West : Direction.Northwest;
        }
        else if (angle >= 90 && angle < 135)
        {
            return angle - 90 >= inbetween ? Direction.Southwest : Direction.West;
        }
        else // angle >= 135
        {
            return angle - 135 >= inbetween ? Direction.South : Direction.Southwest;
        }
    }
    public static Vector2 GetVectorFromDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Vector2.up;
            case Direction.South: return Vector2.down;
            case Direction.West: return Vector2.left;
            case Direction.East: return Vector2.right;
            case Direction.Northeast: return (Vector2.up + Vector2.right).normalized;
            case Direction.Northwest: return (Vector2.up + Vector2.left).normalized;
            case Direction.Southeast: return (Vector2.down + Vector2.right).normalized;
            case Direction.Southwest: return (Vector2.down + Vector2.left).normalized;
            default: return Vector2.zero;
        }
    }
    public static Vector2 GetVectorFromString(string dir)
    {
        switch (dir.ToLower())
        {
            case "north": return Vector2.up;
            case "south": return Vector2.down;
            case "east": return Vector2.right;
            case "west": return Vector2.left;
            case "northeast": return (Vector2.up + Vector2.right).normalized;
            case "northwest": return (Vector2.up + Vector2.left).normalized;
            case "southeast": return (Vector2.down + Vector2.right).normalized;
            case "southwest": return (Vector2.down + Vector2.left).normalized;
            default: return Vector2.down;
        }
    }
    public static Direction GetDirectionFromString(string dir)
    {
        switch (dir.ToLower())
        {
            case "north": return Direction.North;
            case "south": return Direction.South;
            case "east": return Direction.East;
            case "west": return Direction.West;
            case "northeast": return Direction.Northeast;
            case "northwest": return Direction.Northwest;
            case "southeast": return Direction.Southeast;
            case "southwest": return Direction.Southwest;
            default: return Direction.None;
        }
    }
    public static bool MaxAngleBetweenVectors(Vector2 u, Vector2 v, float angle) => Vector2.Angle(u, v) <= angle;
}
