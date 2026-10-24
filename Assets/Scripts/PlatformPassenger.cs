using UnityEngine;

/// <summary>
/// Legacy script. Remove this component from the Player.
/// Do not parent a Dynamic Rigidbody2D to a moving platform.
/// PlayerController2D now handles platform carry.
/// </summary>
[DisallowMultipleComponent]
public class PlatformPassenger : MonoBehaviour
{
    private void Awake()
    {
        Debug.LogWarning("PlatformPassenger устарел: удали этот компонент с Player. Переносом занимается PlayerController2D.", this);
        enabled = false;
    }
}
