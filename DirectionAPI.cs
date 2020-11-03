using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public static class DirectionAPI
{
    ////////////////////////////////////////////////////////////////
    //// THIS API CONTAINS FUNCTIONS RELATED TO DIRECTIONAL IDS ////
    ////////////////////////////////////////////////////////////////


    //Directional ID
    public const int TOP          = 0;
    public const int TOP_RIGHT    = 1;
    public const int RIGHT        = 2;
    public const int BOTTOM_RIGHT = 3;
    public const int BOTTOM       = 4;
    public const int BOTTOM_LEFT  = 5;
    public const int LEFT         = 6;
    public const int TOP_LEFT     = 7;
    public const int CENTER       = 8;
    public const int VERTICAL     = 9;
    public const int HORIZONTAL   = 10;
    public const int ORTHOGONAL   = 11;
    public const int DIAGONAL     = 12;



    /// <summary>
    /// Get the direction clockwise of the inputed direction
    /// </summary>
    /// <param name="startingDirection"> The directional ID of the initial direction </param>
    /// <param name="stepsClockwise"> How many steps clockwise we are searching; if value is negative, then the code will step counter-clockwise for the 
    ///                               number of step equal to the absolute value of the input. </param>
    /// <returns> The directional ID of the searched direction </returns>
    public static int GetClockwiseDirection(int startingDirection, int stepsClockwise = 1)
    {
        int mod = (startingDirection + stepsClockwise) % 8;
        return (mod < 0) ? mod + 8 : mod;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startingDirection"></param>
    /// <returns></returns>
    public static int GetOppositeDirection(int startingDirection)
    {
        return GetClockwiseDirection(startingDirection, 4);
    }


    public static int GetStepsBetweenDirection(int fromDirection, int toDirection)
    {
        int steps = toDirection - fromDirection;
        return steps > 4 ? steps - 8 : steps < -4 ? steps + 8 : steps;
    }

    /// <summary>
    /// Retrieves the direction ID for what direction one position is in relation to another
    /// </summary>
    /// <param name="fromPosition"> The position that serves as that focal point for the direction </param>
    /// <param name="toPosition"> The position whose direction is in relation to fromPosition is the direction we are looking for. </param>
    /// <returns> The direction ID indicating the direction toPosition is in from fromPosition </returns>
    public static int GetDirectionFromPositionToPosition(Vector2Int fromPosition, Vector2Int toPosition)
    {
        bool greaterX = fromPosition.x < toPosition.x;
        bool  lesserX = fromPosition.x > toPosition.x;
        bool greaterY = fromPosition.y < toPosition.y;
        bool  lesserY = fromPosition.y > toPosition.y;

        if (greaterX)
        {
            if ( greaterY && !lesserY) return TOP_RIGHT;
            if (!greaterY && !lesserY) return RIGHT;
            if (!greaterY &&  lesserY) return BOTTOM_RIGHT;
        }
        else if (lesserX)
        {
            if ( greaterY && !lesserY) return TOP_LEFT;
            if (!greaterY && !lesserY) return LEFT;
            if (!greaterY &&  lesserY) return BOTTOM_LEFT;
        }
        else
        {
            if ( greaterY && !lesserY) return TOP;
            if (!greaterY && !lesserY) return CENTER;
            if (!greaterY &&  lesserY) return BOTTOM;
        }
        return -1;
    }
    public static int GetDirectionFromPositionToPosition(Vector3Int fromPosition, Vector3Int toPosition)
    {
        return GetDirectionFromPositionToPosition(new Vector2Int(fromPosition.x, fromPosition.z), new Vector2Int(toPosition.x, toPosition.y));
    }

    public static int GetVerticalDirection(int direction)
    {
        if (direction == HORIZONTAL   || direction == VERTICAL) return direction;
        if (direction == TOP_RIGHT    || direction == TOP    || direction == TOP_LEFT   ) return TOP;
        if (direction == RIGHT        || direction == CENTER || direction == LEFT       ) return HORIZONTAL;
        if (direction == BOTTOM_RIGHT || direction == BOTTOM || direction == BOTTOM_LEFT) return BOTTOM;
        Debug.Log("ERROR: Invalid Direction ID Inputted");
        return -1;
    }

    public static int GetHorizontalDirection(int direction)
    {
        if (direction == HORIZONTAL || direction == VERTICAL) return direction;
        if (direction == TOP_RIGHT  || direction == RIGHT  || direction == BOTTOM_RIGHT) return RIGHT;
        if (direction == TOP        || direction == CENTER || direction == BOTTOM      ) return VERTICAL;
        if (direction == TOP_LEFT   || direction == LEFT   || direction == BOTTOM_LEFT ) return LEFT;
        Debug.Log("ERROR: Invalid Direction ID Inputted");
        return -1;
    }
}
