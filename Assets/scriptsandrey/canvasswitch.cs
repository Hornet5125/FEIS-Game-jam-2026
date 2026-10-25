
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject currentCanvas;
    public GameObject nextCanvas;
    public Button switchButton;

    void Start()
    {
        if (switchButton != null)
            switchButton.onClick.AddListener(SwitchCanvas);
    }

    public void SwitchCanvas()
    {
        if (currentCanvas != null)
            currentCanvas.SetActive(false);

        if (nextCanvas != null)
            nextCanvas.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnDestroy()
    {
        if (switchButton != null)
            switchButton.onClick.RemoveListener(SwitchCanvas);
    }
}
