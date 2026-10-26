
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueScript : MonoBehaviour
{
    public Image dialogueImage;
    public TMP_Text characterName;
    public TMP_Text dialogueText;
    public Image backgroundImage;
    public string fileName = "Scene01";

    private string[] lines;
    private int currentLine = 0;
    private string currentSpeaker = "";
    private Dictionary<string, int> labels = new Dictionary<string, int>();

    public float letterDelay = 0.04f;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string fullText = "";

    public Image fadeImage;
    public float fadeDuration = 0.5f;
    private bool isChangingBackground = false;

    public GameObject choicesPanel;
    public Button choiceButton1;
    public Button choiceButton2;

    private string option1Label;
    private string option2Label;
    private bool isShowingChoices = false;
    private bool isPaused = false;
    private bool isScenarioEnded = false;

    void Start()
    {
        if (choicesPanel != null)
            choicesPanel.SetActive(false);

        LoadScenario();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isChangingBackground || isShowingChoices || isPaused || isScenarioEnded)
                return;

            if (isTyping)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                if (dialogueText != null)
                    dialogueText.text = fullText;

                isTyping = false;
                typingCoroutine = null;
            }
            else
            {
                ReadNextLine();
            }
        }
    }

    void LoadScenario()
    {
        TextAsset file = Resources.Load<TextAsset>(fileName);

        if (file == null)
        {
            Debug.LogError("Не найден TXT в Resources: " + fileName);
            isScenarioEnded = true;
            return;
        }

        lines = file.text.Split(
            new[] { "\r\n", "\n", "\r" },
            StringSplitOptions.None
        );

        currentLine = 0;
        currentSpeaker = "";
        isScenarioEnded = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        FindLabels();
        ReadNextLine();
    }

    void FindLabels()
    {
        labels.Clear();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (line.StartsWith("@label "))
            {
                string labelName = line.Substring("@label ".Length).Trim();

                if (labels.ContainsKey(labelName))
                {
                    Debug.LogError("Повторная метка: " + labelName);
                }
                else
                {
                    labels.Add(labelName, i);
                }
            }
        }
    }

    void ReadNextLine()
    {
        if (lines == null || isScenarioEnded)
            return;

        while (currentLine < lines.Length)
        {
            string line = lines[currentLine].Trim();
            currentLine++;

            if (string.IsNullOrEmpty(line))
                continue;

            if (line.StartsWith("@scene "))
                continue;

            if (line.StartsWith("@label "))
                continue;

            if (line == "@choice")
            {
                ShowChoices();
                return;
            }

            if (line.StartsWith("@name "))
            {
                currentSpeaker = line.Substring("@name ".Length).Trim();
                continue;
            }

            if (line.StartsWith("@background "))
            {
                ChangeBackground(
                    line.Substring("@background ".Length).Trim()
                );

                continue;
            }

            if (line.StartsWith("@jump "))
            {
                string target = line.Substring("@jump ".Length).Trim();

                if (JumpTo(target))
                    continue;

                return;
            }

            if (line == "@hideUI")
            {
                HideUI();
                continue;
            }

            if (line == "@showUI")
            {
                ShowUI();
                continue;
            }

            if (line == "@Pause" || line.StartsWith("@Pause "))
            {
                float seconds = 5f;
                string argument = line.Substring("@Pause".Length).Trim();
                if (!string.IsNullOrEmpty(argument))
                {
                    if (!float.TryParse(argument, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out seconds))
                        seconds = 5f;
                }
                StartCoroutine(PauseForSeconds(Mathf.Max(0f, seconds)));
                return;
            }

            if (line.StartsWith("@end"))
            {
                string sceneName = line.Substring("@end".Length).Trim();

                if (string.IsNullOrEmpty(sceneName))
                {
                    isScenarioEnded = true;

                    if (characterName != null)
                        characterName.text = "";

                    if (dialogueText != null)
                        dialogueText.text = "Конец игры.";

                    Debug.Log("Сценарий завершён.");
                    return;
                }

                if (!Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    Debug.LogError(
                        "Не удалось загрузить Unity-сцену: " + sceneName +
                        ". Проверь имя сцены и список сцен сборки."
                    );
                    return;
                }

                Debug.Log("Переход на Unity-сцену: " + sceneName);
                SceneManager.LoadScene(sceneName);
                return;
            }

            if (line.StartsWith("@narration "))
            {
                ShowText(
                    "",
                    line.Substring("@narration ".Length).Trim()
                );
                return;
            }

            if (line.StartsWith("\"") &&
                line.EndsWith("\"") &&
                line.Length >= 2)
            {
                string text = line.Substring(1, line.Length - 2);
                ShowText(currentSpeaker, text);
                return;
            }

            Debug.LogWarning("Неизвестная команда в TXT: " + line);
        }

        isScenarioEnded = true;

        if (characterName != null)
            characterName.text = "";

        if (dialogueText != null)
            dialogueText.text = "Сценарий завершён.";
    }

    void ShowText(string speaker, string text)
    {
        if (characterName != null)
            characterName.text = speaker;

        if (dialogueText == null)
            return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        fullText = text;
        dialogueText.text = "";

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;

        foreach (char letter in fullText)
        {
            if (dialogueText == null)
                break;

            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        if (dialogueText != null)
            dialogueText.text = fullText;

        isTyping = false;
        typingCoroutine = null;
    }

    bool JumpTo(string labelName)
    {
        if (labels.TryGetValue(labelName, out int index))
        {
            currentLine = index + 1;
            return true;
        }

        Debug.LogError("Метка не найдена: " + labelName);
        return false;
    }

    void ChangeBackground(string backgroundName)
    {
        if (backgroundImage == null)
        {
            Debug.LogError("Не подключён Background Image в Inspector!");
            return;
        }

        Sprite sprite = Resources.Load<Sprite>(
            "DialogueImages/" + backgroundName
        );

        if (sprite == null)
        {
            Debug.LogError(
                "Картинка не найдена: Resources/DialogueImages/" +
                backgroundName
            );
            return;
        }

        if (!isChangingBackground)
            StartCoroutine(ChangeBackgroundWithFade(sprite));
    }

    void ShowChoices()
{
    if (choicesPanel == null ||
        choiceButton1 == null ||
        choiceButton2 == null)
    {
        Debug.LogError("Не подключены элементы выбора в Inspector!");
        return;
    }

    List<string> optionTexts = new List<string>();
    List<string> optionLabels = new List<string>();

    while (currentLine < lines.Length)
    {
        string line = lines[currentLine].Trim();

        if (!line.StartsWith("@option "))
            break;

        currentLine++;

        string option = line.Substring("@option ".Length);
        string[] parts = option.Split('|');

        if (parts.Length != 2)
        {
            Debug.LogError("Неверный формат @option: " + line);
            continue;
        }

        optionTexts.Add(parts[0].Trim());
        optionLabels.Add(parts[1].Trim());
    }

    if (optionTexts.Count != 2)
    {
        Debug.LogError("Необходимо указать ровно два варианта выбора.");
        return;
    }

    option1Label = optionLabels[0];
    option2Label = optionLabels[1];

    TMP_Text text1 = choiceButton1.GetComponentInChildren<TMP_Text>(true);
    TMP_Text text2 = choiceButton2.GetComponentInChildren<TMP_Text>(true);

    if (text1 != null)
        text1.text = optionTexts[0];

    if (text2 != null)
        text2.text = optionTexts[1];

    choiceButton1.onClick.RemoveAllListeners();
    choiceButton2.onClick.RemoveAllListeners();

    choiceButton1.interactable = true;
    choiceButton2.interactable = true;

    choiceButton1.onClick.AddListener(() =>
    {
        Debug.Log("Нажат первый вариант: " + option1Label);
        SelectChoice(option1Label);
    });

    choiceButton2.onClick.AddListener(() =>
    {
        Debug.Log("Нажат второй вариант: " + option2Label);
        SelectChoice(option2Label);
    });

    CanvasGroup group = choicesPanel.GetComponent<CanvasGroup>();

    if (group == null)
        group = choicesPanel.AddComponent<CanvasGroup>();

    group.alpha = 1f;
    group.interactable = true;
    group.blocksRaycasts = true;
    group.ignoreParentGroups = false;

    choicesPanel.SetActive(true);
    isShowingChoices = true;

    Debug.Log("Панель выбора показана. Кнопки активны.");
}

    void SelectChoice(string targetLabel)
    {
        choicesPanel.SetActive(false);
        isShowingChoices = false;

        if (JumpTo(targetLabel))
            ReadNextLine();
    }

    IEnumerator ChangeBackgroundWithFade(Sprite newBackground)
    {
        isChangingBackground = true;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);

            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            while (color.a < 1f)
            {
                color.a = Mathf.Clamp01(
                    color.a + Time.deltaTime / Mathf.Max(fadeDuration, 0.01f)
                );

                fadeImage.color = color;
                yield return null;
            }
        }

        backgroundImage.sprite = newBackground;

        if (fadeImage != null)
        {
            Color color = fadeImage.color;

            while (color.a > 0f)
            {
                color.a = Mathf.Clamp01(
                    color.a - Time.deltaTime / Mathf.Max(fadeDuration, 0.01f)
                );

                fadeImage.color = color;
                yield return null;
            }

            color.a = 0f;
            fadeImage.color = color;
            fadeImage.gameObject.SetActive(false);
        }

        isChangingBackground = false;
    }

    void HideUI()
    {
        if (dialogueImage != null)
            dialogueImage.gameObject.SetActive(false);

        if (characterName != null)
            characterName.gameObject.SetActive(false);

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);

        Debug.Log("Выполнена команда @hideUI");
    }

    void ShowUI()
    {
        if (dialogueImage != null)
            dialogueImage.gameObject.SetActive(true);

        if (characterName != null)
            characterName.gameObject.SetActive(true);

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(true);

        Debug.Log("Интерфейс диалога показан.");
    }

    IEnumerator PauseForSeconds(float seconds)
    {
        isPaused = true;

        yield return new WaitForSecondsRealtime(seconds);

        isPaused = false;

        ReadNextLine();
    }
}
