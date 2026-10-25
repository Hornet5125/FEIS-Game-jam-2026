
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight")]
    [SerializeField] private Light flashlight;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    [Header("Flicker")]
    [SerializeField] private float minIntensity = 7.5f;
    [SerializeField] private float maxIntensity = 10f;
    [SerializeField] private float flickerChance = 0.12f;
    [SerializeField] private float flickerDuration = 0.08f;
    [SerializeField] private float flickerIntervalMin = 2f;
    [SerializeField] private float flickerIntervalMax = 7f;

    [Header("Movement Sway")]
    [SerializeField] private float swayAngle = 1.5f;
    [SerializeField] private float swaySpeed = 7f;
    [SerializeField] private float positionSway = 0.025f;
    [SerializeField] private float smoothness = 8f;

    private CharacterController playerController;

    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private float baseIntensity;
    private float nextFlickerTime;
    private float flickerEndTime;
    private float flickerMultiplier = 1f;

    private bool isOn = true;
    private float movementTime;

    private void Awake()
    {
        if (flashlight == null)
            flashlight = GetComponent<Light>();

        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;

        baseIntensity = flashlight.intensity;

        // Ищем CharacterController у родительского игрока.
        playerController =
            GetComponentInParent<CharacterController>();

        flashlight.enabled = true;
        ScheduleNextFlicker();
    }

    private void Update()
    {
        HandleToggle();
        HandleFlicker();
        HandleMovementSway();
    }

    private void HandleToggle()
    {
        if (!Input.GetKeyDown(toggleKey))
            return;

        isOn = !isOn;
        flashlight.enabled = isOn;

        if (isOn)
        {
            flickerMultiplier = 1f;
            flashlight.intensity = baseIntensity;
            ScheduleNextFlicker();
        }
    }

    private void HandleFlicker()
    {
        if (!isOn)
            return;

        if (Time.time >= flickerEndTime &&
            flickerMultiplier < 1f)
        {
            flickerMultiplier = 1f;
            flashlight.intensity = baseIntensity;
            ScheduleNextFlicker();
        }

        if (Time.time >= nextFlickerTime &&
            flickerMultiplier >= 1f)
        {
            if (Random.value < flickerChance)
            {
                // Краткий провал яркости, но не полное выключение.
                flickerMultiplier = Random.Range(0.55f, 0.85f);
                flickerEndTime = Time.time + flickerDuration;
                flashlight.intensity =
                    baseIntensity * flickerMultiplier;
            }
            else
            {
                ScheduleNextFlicker();
            }
        }
    }

    private void ScheduleNextFlicker()
    {
        nextFlickerTime = Time.time +
            Random.Range(flickerIntervalMin, flickerIntervalMax);
    }

    private void HandleMovementSway()
    {
        float movementAmount = 0f;

        if (playerController != null)
        {
            Vector3 velocity = playerController.velocity;
            velocity.y = 0f;

            movementAmount = Mathf.Clamp01(
                velocity.magnitude / 4f
            );
        }

        movementTime += Time.deltaTime * swaySpeed;

        // Покачивание усиливается при движении.
        float horizontalSway =
            Mathf.Sin(movementTime) * swayAngle * movementAmount;

        float verticalSway =
            Mathf.Sin(movementTime * 2f) *
            swayAngle * 0.6f * movementAmount;

        Quaternion targetRotation =
            initialRotation *
            Quaternion.Euler(verticalSway, horizontalSway, 0f);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * smoothness
        );

        Vector3 targetPosition = initialPosition +
            new Vector3(
                Mathf.Sin(movementTime) * positionSway * movementAmount,
                Mathf.Abs(Mathf.Sin(movementTime * 2f)) *
                    positionSway * movementAmount,
                0f
            );

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * smoothness
        );
    }
}
