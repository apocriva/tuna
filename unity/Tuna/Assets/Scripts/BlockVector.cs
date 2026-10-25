using System;
using Unity.Mathematics;
using UnityEngine;

public enum WorldDirection
{
    North,
    West,
    South,
    East,
}

public enum Direction
{
    Forward,
    Left,
    Back,
    Right,
}

public static class DirectionUtility
{
    public static WorldDirection WorldFromHeading(float heading)
    {
        return (WorldDirection)FromAngle(heading);
    }

    public static float ToHeading(WorldDirection direction)
    {
        return direction switch
        {
            WorldDirection.North => 0f,
            WorldDirection.West => 270f,
            WorldDirection.South => 180f,
            WorldDirection.East => 90f,
            _ => throw new ArgumentException()
        };
    }

    public static Direction FromAngle(float angle)
    {
        float angle360 = angle - (Mathf.Floor(angle / 360f) * 360f);

        return angle360 switch
        {
            >= 315f or < 45f => Direction.Forward,
            >= 45f and < 135f => Direction.Right,
            >= 135f and < 225f => Direction.Back,
            >= 225f and < 315f => Direction.Left,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

public static class BlockVectorExtensions
{
    public const float WorldToBlockPosRatio = 1.0f;

    // WorldDirections
    public static readonly int3 North = new(0, 0, 1);
    public static readonly int3 East = new(1, 0, 0);
    public static readonly int3 South = new(0, 0, -1);
    public static readonly int3 West = new(-1, 0, 0);

    // Directions
    public static readonly int3 Forward = new(0, 0, 1);
    public static readonly int3 Right = Forward.ToDirection(Direction.Right);
    public static readonly int3 Back = Forward.ToDirection(Direction.Back);
    public static readonly int3 Left = Forward.ToDirection(Direction.Left);

    public static int3 FromWorld(Vector3 world)
    {
        return new((int)(world.x / WorldToBlockPosRatio), (int)(world.y / WorldToBlockPosRatio), (int)(world.z / WorldToBlockPosRatio));
    }

    public static Vector3 ToWorld(this int3 i3)
    {
        return new(i3.x * WorldToBlockPosRatio, i3.y * WorldToBlockPosRatio, i3.z * WorldToBlockPosRatio);
    }

    /// <summary>
    /// Returns this <see cref="int3"> rotated to the specified direction,
    /// where +Y is forward and +X is right.
    /// </summary>
    public static int3 ToDirection(this int3 i3, Direction direction) => direction switch
    {
        Direction.Forward => new(i3.x, i3.y, i3.z),
        Direction.Right => new(i3.z, i3.y, -i3.x),
        Direction.Back => new(-i3.x, i3.y, -i3.z),
        Direction.Left => new(-i3.z, i3.y, i3.x),
        _ => throw new ArgumentOutOfRangeException()
    };

    public static int3 FromWorldDirection(WorldDirection direction) => direction switch
    {
        WorldDirection.North => North,
        WorldDirection.East => East,
        WorldDirection.South => South,
        WorldDirection.West => West,
        _ => throw new ArgumentOutOfRangeException()
    };

    public static WorldDirection RelativeTo(this Direction relative, WorldDirection direction)
    {
        return (WorldDirection)(((int)direction + (int)relative) % 4);
    }

    public static int3 ToInt3(this WorldDirection direction) => FromWorldDirection(direction);

    public static bool Any(this bool3 b3)
    {
        return b3.x || b3.y || b3.z;
    }
}