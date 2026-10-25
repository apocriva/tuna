using TilemapCreator3D;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Current;

    public Tilemap3D Tilemap;

    protected void Awake()
    {
        Current = this;
    }
}
