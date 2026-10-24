
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private SpriteRenderer doorRenderer;

    private bool isOpen;

    private void Awake()
    {
        if (doorCollider == null)
            doorCollider = GetComponent<Collider2D>();

        if (doorRenderer == null)
            doorRenderer = GetComponent<SpriteRenderer>();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (doorCollider != null)
            doorCollider.enabled = !isOpen;

        if (doorRenderer != null)
            doorRenderer.enabled = !isOpen;

        Debug.Log(isOpen ? "Door opened!" : "Door closed!");
    }
}
