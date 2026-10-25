using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsPanel;
    public Button openButton;
    public Button closeButton;

    [Header("Behavior")]
    public bool pauseGame = false;
    public bool closeOnEscape = false;

    private void Awake()
    {
        if (openButton != null)
            openButton.onClick.AddListener(OpenMenu);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMenu);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        else
            Debug.LogError("Settings: Settings Panel is not assigned", this);
    }

    private void Update()
    {
        if (closeOnEscape && settingsPanel != null && settingsPanel.activeSelf &&
            Input.GetKeyDown(KeyCode.Escape))
            CloseMenu();
    }

    public void OpenMenu()
    {
        if (settingsPanel == null) return;
        settingsPanel.SetActive(true);
        if (pauseGame) Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Settings: opened", this);
    }

    public void CloseMenu()
    {
        if (settingsPanel == null) return;
        // Ensure the legacy dropdown's temporary blocker is removed first.
        Dropdown[] dropdowns = settingsPanel.GetComponentsInChildren<Dropdown>(true);
        foreach (Dropdown dropdown in dropdowns)
            dropdown.Hide();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
        settingsPanel.SetActive(false);
        if (pauseGame) Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Settings: closed", this);
    }

    private void OnDestroy()
    {
        if (openButton != null)
            openButton.onClick.RemoveListener(OpenMenu);
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseMenu);
    }
}
