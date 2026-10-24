using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [Tooltip("Панель меню настроек (объект, который нужно показывать/скрывать)")]
    public GameObject settingsPanel;

    [Tooltip("Кнопка открытия меню (опционально)")]
    public Button openButton;

    [Tooltip("Кнопка закрытия меню (крестик внутри панели, опционально)")]
    public Button closeButton;

    [Header("Настройки поведения")]
    [Tooltip("Ставить ли игру на паузу при открытии меню")]
    public bool pauseGame = true;

    [Tooltip("Закрывать меню по клавише Escape")]
    public bool closeOnEscape = true;

    [Tooltip("Показывать ли курсор при открытом меню")]
    public bool showCursor = true;

    private bool isOpen = false;

    void Start()
    {
        // Скрываем панель при старте
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Подписываемся на кнопки, если они назначены
        if (openButton != null)
            openButton.onClick.AddListener(OpenMenu);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMenu);
    }

    void Update()
    {
        // Закрытие по Escape
        if (closeOnEscape && isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        if (isOpen) return;

        isOpen = true;
        settingsPanel.SetActive(true);

        if (pauseGame)
            Time.timeScale = 0f; // Пауза

        if (showCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void CloseMenu()
    {
        if (!isOpen) return;

        isOpen = false;
        settingsPanel.SetActive(false);

        if (pauseGame)
            Time.timeScale = 1f; // Снимаем паузу

        if (showCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Публичный метод для переключения (удобно вешать на одну кнопку)
    public void ToggleMenu()
    {
        if (isOpen) CloseMenu();
        else OpenMenu();
    }

    void OnDestroy()
    {
        // Отписываемся от событий, чтобы избежать утечек
        if (openButton != null)
            openButton.onClick.RemoveListener(OpenMenu);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseMenu);
    }
}