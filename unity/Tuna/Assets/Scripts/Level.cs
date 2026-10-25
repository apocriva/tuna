using TilemapCreator3D;
using Unity.Mathematics;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Current;

    public Tilemap3D Tilemap;

    protected void Awake()
    {
        Current = this;
    }

    public PassageType GetPassageBetween(int3 source, int3 target)
    {
        // Source and target must be adjacent!
        var delta = math.abs(target - source);

        if (delta.x > 0 && (delta.y > 0 || delta.z > 0) ||
            delta.y > 0 && (delta.x > 0 || delta.z > 0) ||
            delta.z > 0 && (delta.x > 0 || delta.y > 0))
        {
            Debug.Assert(false, $"Passage can only be calculated between neighbours.");
            return PassageType.None;
        }

        if ((target < int3.zero).Any() ||
            (target >= Tilemap.Size).Any())
        {
            // Out of bounds.
            return PassageType.None;
        }

        return PassageType.Walkable;
    }
}
