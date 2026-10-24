
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Vector3 offset = new Vector3(2f, 1f, -10f);

    [Header("Level Bounds")]
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform topRight;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;

        targetPosition = ClampCameraPosition(targetPosition);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            1f - Mathf.Exp(-smoothSpeed * Time.deltaTime)
        );
    }

    private Vector3 ClampCameraPosition(Vector3 position)
    {
        if (bottomLeft == null || topRight == null || cam == null)
            return position;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = bottomLeft.position.x + halfWidth;
        float maxX = topRight.position.x - halfWidth;
        float minY = bottomLeft.position.y + halfHeight;
        float maxY = topRight.position.y - halfHeight;

        // If the level is smaller than the camera view, center on the level.
        position.x = minX > maxX
            ? (bottomLeft.position.x + topRight.position.x) / 2f
            : Mathf.Clamp(position.x, minX, maxX);

        position.y = minY > maxY
            ? (bottomLeft.position.y + topRight.position.y) / 2f
            : Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
