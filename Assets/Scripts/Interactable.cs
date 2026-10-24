
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private string objectName = "Unknown object";
    [SerializeField, TextArea] private string message = "Nothing happened.";

    [Header("Optional Switch")]
    [SerializeField] private Switch targetSwitch;

    public void Interact()
    {
        if (targetSwitch != null)
        {
            targetSwitch.Interact();
            return;
        }

        Debug.Log(objectName + ": " + message);
    }
}
