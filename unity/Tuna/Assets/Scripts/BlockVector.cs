using System;
using UnityEngine;

public enum WorldDirection
{
    North,
    East,
    South,
    West,
}

public enum Direction
{
    Forward,
    Right,
    Back,
    Left,
}

public static class DirectionUtility
{
    public static WorldDirection WorldFromHeading(float heading)
    {
        return (WorldDirection)FromAngle(heading);
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

    public static WorldDirection RelativeTo(this Direction relative, WorldDirection direction)
    {
        return (WorldDirection)(((int)direction + (int)relative) % 4);
    }

    public static BlockVector ToBlockVector(this WorldDirection direction) => BlockVector.FromWorldDirection(direction);
}

public struct BlockVector : IEquatable<BlockVector>
{
    public const float WorldToBlockPosRatio = 1.0f;

    // WorldDirections
    public static readonly BlockVector North = new(0, 1);
    public static readonly BlockVector East = new(1, 0);
    public static readonly BlockVector South = new(0, -1);
    public static readonly BlockVector West = new(-1, 0);

    // Directions
    public static readonly BlockVector Forward = new(0, 1);
    public static readonly BlockVector Right = Forward.ToDirection(Direction.Right);
    public static readonly BlockVector Back = Forward.ToDirection(Direction.Back);
    public static readonly BlockVector Left = Forward.ToDirection(Direction.Left);

    public int X { get; private set; }
    public int Y { get; private set; }

    public BlockVector(int x, int y)
    {
        X = x;
        Y = y;
    }

    public readonly bool Equals(BlockVector other)
    {
        return X == other.X && Y == other.Y;
    }

    public static BlockVector operator +(BlockVector a, BlockVector b)
    {
        return new(a.X + b.X, a.Y + b.Y);
    }

    public static BlockVector operator -(BlockVector a, BlockVector b)
    {
        return new(a.X - b.X, a.Y - b.Y);
    }

    public readonly Vector3 ToWorld()
    {
        return new(X * WorldToBlockPosRatio, 0f, Y * WorldToBlockPosRatio);
    }

    /// <summary>
    /// Returns this <see cref="BlockVector"> rotated to the specified direction,
    /// where +Y is forward and +X is right.
    /// </summary>
    public readonly BlockVector ToDirection(Direction direction) => direction switch
    {
        Direction.Forward => new(X, Y),
        Direction.Right => new(Y, -X),
        Direction.Back => new(-X, -Y),
        Direction.Left => new(-Y, X),
        _ => throw new ArgumentOutOfRangeException()
    };

    public static BlockVector FromWorld(Vector3 world)
    {
        return new((int)(world.x / WorldToBlockPosRatio), (int)(world.z / WorldToBlockPosRatio));
    }

    public static BlockVector FromWorldDirection(WorldDirection direction) => direction switch
    {
        WorldDirection.North => North,
        WorldDirection.East => East,
        WorldDirection.South => South,
        WorldDirection.West => West,
        _ => throw new ArgumentOutOfRangeException()
    };
}