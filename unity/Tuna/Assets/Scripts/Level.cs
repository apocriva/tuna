using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Current;

    protected void Awake()
    {
        Current = this;
    }
}
