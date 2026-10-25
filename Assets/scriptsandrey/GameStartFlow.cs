using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartFlow : MonoBehaviour
{
    [Header("Panels / Canvas objects")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject nameMenu;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;

    [Header("Player name")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorText;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "game";

    private const string NameKey = "CharacterName";

    public static string CharacterName => PlayerPrefs.GetString(NameKey, "");

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (mainMenu != null) mainMenu.SetActive(true);
        if (nameMenu != null) nameMenu.SetActive(false);
        if (errorText != null) errorText.gameObject.SetActive(false);

        if (nameInput != null)
        {
            nameInput.SetTextWithoutNotify(CharacterName);
            nameInput.onValueChanged.AddListener(OnNameChanged);
        }

        if (playButton != null) playButton.onClick.AddListener(OpenNameMenu);
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmNameAndStart);
        if (backButton != null) backButton.onClick.AddListener(BackToMainMenu);
        UpdateConfirmButton();
    }

    private void OpenNameMenu()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (nameMenu != null) nameMenu.SetActive(true);
        if (errorText != null) errorText.gameObject.SetActive(false);
        UpdateConfirmButton();
        if (nameInput != null) nameInput.Select();
    }

    private void BackToMainMenu()
    {
        if (nameMenu != null) nameMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
        if (errorText != null) errorText.gameObject.SetActive(false);
    }

    private void OnNameChanged(string value)
    {
        UpdateConfirmButton();
        if (errorText != null) errorText.gameObject.SetActive(false);
    }

    private void UpdateConfirmButton()
    {
        if (confirmButton != null)
            confirmButton.interactable = nameInput != null && !string.IsNullOrWhiteSpace(nameInput.text);
    }

    public void ConfirmNameAndStart()
    {
        string newName = nameInput != null ? nameInput.text.Trim() : "";
        if (string.IsNullOrWhiteSpace(newName))
        {
            ShowError("Введите имя персонажа!");
            UpdateConfirmButton();
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            ShowError("Сцена '" + gameSceneName + "' не добавлена в Build Profiles.");
            return;
        }

        PlayerPrefs.SetString(NameKey, newName);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }
        Debug.LogWarning(message);
    }

    private void OnDestroy()
    {
        if (playButton != null) playButton.onClick.RemoveListener(OpenNameMenu);
        if (confirmButton != null) confirmButton.onClick.RemoveListener(ConfirmNameAndStart);
        if (backButton != null) backButton.onClick.RemoveListener(BackToMainMenu);
        if (nameInput != null) nameInput.onValueChanged.RemoveListener(OnNameChanged);
    }
}
