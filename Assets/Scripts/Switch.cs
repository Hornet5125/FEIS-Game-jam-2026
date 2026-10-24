
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    [Header("Switch Settings")]
    [SerializeField] private UnityEvent onActivate;
    [SerializeField] private UnityEvent onDeactivate;

    private bool isActivated;

    public void Interact()
    {
        isActivated = !isActivated;

        if (isActivated)
        {
            onActivate.Invoke();
            Debug.Log("Switch activated!");
        }
        else
        {
            onDeactivate.Invoke();
            Debug.Log("Switch deactivated!");
        }
    }
}
