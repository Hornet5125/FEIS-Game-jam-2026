
using UnityEngine;

public class FogMovement : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool followVertical = false;

    private void Update()
    {
        if (player == null)
            return;

        Vector3 position = transform.position;

        position.x = Mathf.MoveTowards(
            position.x,
            player.position.x,
            moveSpeed * Time.deltaTime
        );

        if (followVertical)
        {
            position.y = Mathf.MoveTowards(
                position.y,
                player.position.y,
                moveSpeed * Time.deltaTime
            );
        }

        transform.position = position;
    }

    public void Initialize(Transform target, float speed)
    {
        player = target;
        moveSpeed = speed;
    }
}
