
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float deathY = -10f;

    private Vector3 spawnPosition;
    private Rigidbody2D rb;
    private PlayerController2D controller;

    private void Awake()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        if (transform.position.y < deathY)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = spawnPosition;

        if (controller != null)
        {
            controller.enabled = false;
            controller.enabled = true;
        }
    }
}
