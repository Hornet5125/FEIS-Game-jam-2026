
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterNameManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button confirmButton;
    public TMP_Text errorText;

    private const string NameKey = "CharacterName";

    public static string CharacterName
    {
        get
        {
            return PlayerPrefs.GetString(NameKey, "");
        }
    }

    void Start()
    {
        nameInput.text = CharacterName;

        nameInput.onValueChanged.AddListener(OnNameChanged);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(SaveName);

        if (errorText != null)
            errorText.gameObject.SetActive(false);

        OnNameChanged(nameInput.text);
    }

    void OnNameChanged(string value)
    {
        bool valid = !string.IsNullOrWhiteSpace(value);

        if (confirmButton != null)
            confirmButton.interactable = valid;

        if (errorText != null)
            errorText.gameObject.SetActive(false);
    }

    public void SaveName()
    {
        string characterName = nameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(characterName))
        {
            if (errorText != null)
            {
                errorText.text = "Введите имя персонажа!";
                errorText.gameObject.SetActive(true);
            }

            return;
        }

        PlayerPrefs.SetString(NameKey, characterName);
        PlayerPrefs.Save();

        Debug.Log("Имя персонажа: " + characterName);
    }
}
