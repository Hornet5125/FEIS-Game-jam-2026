
using UnityEngine;

public class PlatformPassenger : MonoBehaviour
{
    private Transform currentPlatform;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.transform;
            transform.SetParent(currentPlatform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (currentPlatform != null &&
            collision.transform == currentPlatform)
        {
            transform.SetParent(null);
            currentPlatform = null;
        }
    }
}
