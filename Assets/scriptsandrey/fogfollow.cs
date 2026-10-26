
using UnityEngine;

public class FogMovementt : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float maxDistanceBehind = 10f;
    [SerializeField] private bool followPlayerHeight = false;
    [SerializeField] private float verticalSpeed = 2f;

public void Initialize(Transform target, float speed)
{
    player = target;
    moveSpeed = speed;
}

    private void Update()
    {
        if (player == null)
            return;

        Vector3 position = transform.position;

        position.x += moveSpeed * Time.deltaTime;

        float minimumX = player.position.x - maxDistanceBehind;

        if (position.x < minimumX)
            position.x = minimumX;

        if (followPlayerHeight)
        {
            position.y = Mathf.MoveTowards(
                position.y,
                player.position.y,
                verticalSpeed * Time.deltaTime
            );
        }

        transform.position = position;
    }
}
