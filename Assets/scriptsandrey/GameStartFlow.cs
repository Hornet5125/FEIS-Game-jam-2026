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
    [SerializeField] private Button continueButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;

    [Header("Player name")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorText;

    private const string GameSceneName = "DoPlatforrmera";

    private const string NameKey = "CharacterName";
    private const string HasGameKey = "HasStartedGame";

    public static string CharacterName => PlayerPrefs.GetString(NameKey, "");
    public static bool HasStartedGame => PlayerPrefs.GetInt(HasGameKey, 0) == 1;

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
            nameInput.SetTextWithoutNotify("");
            nameInput.onValueChanged.AddListener(OnNameChanged);
        }

        if (playButton != null) playButton.onClick.AddListener(OpenNameMenu);
        if (continueButton != null) continueButton.onClick.AddListener(ContinueGame);
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmNameAndStart);
        if (backButton != null) backButton.onClick.AddListener(BackToMainMenu);

        UpdateConfirmButton();
        UpdateContinueButton();
    }

    private void OpenNameMenu()
    {
        if (nameInput != null) nameInput.SetTextWithoutNotify("");
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
        UpdateContinueButton();
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

    private void UpdateContinueButton()
    {
        if (continueButton != null)
            continueButton.interactable = HasStartedGame && !string.IsNullOrWhiteSpace(CharacterName);
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

        if (!CheckGameScene()) return;

        PlayerPrefs.SetString(NameKey, newName);
        PlayerPrefs.SetInt(HasGameKey, 1);
        PlayerPrefs.Save();
        LoadGameScene();
    }

    public void ContinueGame()
    {
        if (!HasStartedGame || string.IsNullOrWhiteSpace(CharacterName))
        {
            ShowError("Нет ранее начатой игры.");
            UpdateContinueButton();
            return;
        }

        if (!CheckGameScene()) return;
        LoadGameScene();
    }

    private bool CheckGameScene()
    {
        if (Application.CanStreamedLevelBeLoaded(GameSceneName)) return true;
        ShowError("Сцена '" + GameSceneName + "' не добавлена в Build Profiles.");
        return false;
    }

    private void LoadGameScene()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
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
        if (continueButton != null) continueButton.onClick.RemoveListener(ContinueGame);
        if (confirmButton != null) confirmButton.onClick.RemoveListener(ConfirmNameAndStart);
        if (backButton != null) backButton.onClick.RemoveListener(BackToMainMenu);
        if (nameInput != null) nameInput.onValueChanged.RemoveListener(OnNameChanged);
    }
}
