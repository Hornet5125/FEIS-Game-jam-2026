
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private Door targetDoor;

    private bool isActivated;

    public void Interact()
    {
        if (targetDoor == null)
        {
            Debug.LogWarning("Switch: Door is not assigned!");
            return;
        }

        targetDoor.ToggleDoor();

        isActivated = !isActivated;

        Debug.Log(isActivated
            ? "Switch activated!"
            : "Switch deactivated!");
    }
}
