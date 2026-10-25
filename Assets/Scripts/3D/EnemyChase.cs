
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform sprite;

    [Header("Movement")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float stopDistance = 1.2f;
    [SerializeField] private float updateRate = 0.15f;

    [Header("Sprite")]
    [SerializeField] private float rotationOffset = 180f;

    private NavMeshAgent agent;
    private float nextUpdateTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed;
        agent.stoppingDistance = stopDistance;
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (player == null)
            return;

        // Обновляем цель регулярно, чтобы преследовать игрока.
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateRate;

            if (agent.isOnNavMesh)
                agent.SetDestination(player.position);
        }

        // Поворачиваем только картинку, а не корень врага.
        if (sprite != null)
        {
            Vector3 direction = player.position - sprite.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                sprite.rotation = Quaternion.LookRotation(direction)
                    * Quaternion.Euler(0f, rotationOffset, 0f);
            }
        }
    }
}
