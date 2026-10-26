using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    void Start()
    {
        LoadScenario();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isChangingBackground || isShowingChoices)
                return;

            if (isTyping)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

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
            Debug.LogError(
                "Не найден TXT в Resources: " + fileName
            );
            return;
        }

        lines = file.text.Split(
            new[] { "\r\n", "\n", "\r" },
            StringSplitOptions.None
        );

        FindLabels();

        currentLine = 0;
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
                string labelName =
                    line.Substring("@label ".Length).Trim();

                if (labels.ContainsKey(labelName))
                {
                    Debug.LogError(
                        "Повторная метка: " + labelName
                    );
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
        if (lines == null)
            return;

        while (currentLine < lines.Length)
        {
            string line = lines[currentLine].Trim();
            currentLine++;

            if (string.IsNullOrEmpty(line))
                continue;

            // Метаданные сцены
            if (line.StartsWith("@scene "))
                continue;

            // Метка — просто точка назначения
            if (line.StartsWith("@label "))
                continue;

            if (line == "@choice")
            {
                ShowChoices();
                return;
            }

            // Имя следующего говорящего
            if (line.StartsWith("@name "))
            {
                currentSpeaker =
                    line.Substring("@name ".Length).Trim();
                continue;
            }

            // Смена фона
            if (line.StartsWith("@background "))
            {
                ChangeBackground(
                    line.Substring("@background ".Length).Trim()
                );
                continue;
            }

            // Переход к другой метке
            if (line.StartsWith("@jump "))
            {
                string target =
                    line.Substring("@jump ".Length).Trim();

                if (JumpTo(target))
                    continue;
                else
                    return;
            }

            if (line == "@hideUI")
            {
                HideUI();
                continue;
            }

            // Конец сценария
            if (line == "@end")
            {
                if (characterName != null)
                    characterName.text = "";

                if (dialogueText != null)
                    dialogueText.text = "Конец сцены.";

                Debug.Log("Сценарий завершён.");
                return;
            }

            // Описание событий от автора
            if (line.StartsWith("@narration "))
            {
                ShowText(
                    " ",
                    line.Substring("@narration ".Length).Trim()
                );
                return;
            }

            // Обычная строка в кавычках — реплика персонажа
            if (line.StartsWith("\"") && line.EndsWith("\"")
                && line.Length >= 2)
            {
                string text = line.Substring(
                    1, line.Length - 2
                );

                ShowText(currentSpeaker, text);
                return;
            }

            Debug.LogWarning(
                "Неизвестная команда в TXT: " + line
            );
        }


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

        // Останавливаем предыдущее печатание, если оно ещё идёт
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        fullText = text;
        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeText());
    }

    System.Collections.IEnumerator TypeText()
    {
        isTyping = true;

        foreach (char letter in fullText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        dialogueText.text = fullText;
        isTyping = false;
        typingCoroutine = null;
    }

    bool JumpTo(string labelName)
    {
        if (labels.TryGetValue(labelName, out int index))
        {
            // Следующая команда после найденной метки
            currentLine = index + 1;
            return true;
        }

        Debug.LogError("Метка не найдена: " + labelName);
        return false;
    }

    void ChangeBackground(string backgroundName)
    {
        Debug.Log("Команда смены фона: " + backgroundName);

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
                "Картинка не найдена: Resources/DialogueImages/"
                + backgroundName
            );
            return;
        }

        // Запускаем плавное затемнение и смену фона
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

        // Собираем следующие команды @option
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
            Debug.LogError("Пока поддерживаются ровно два варианта выбора.");
            return;
        }

        option1Label = optionLabels[0];
        option2Label = optionLabels[1];

        choiceButton1.GetComponentInChildren<TMP_Text>().text = optionTexts[0];
        choiceButton2.GetComponentInChildren<TMP_Text>().text = optionTexts[1];

        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        choiceButton1.onClick.AddListener(() => SelectChoice(option1Label));
        choiceButton2.onClick.AddListener(() => SelectChoice(option2Label));


        CanvasGroup group = choicesPanel.GetComponent<CanvasGroup>();

        if (group == null)
        {
            group = choicesPanel.AddComponent<CanvasGroup>();
        }
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        choicesPanel.SetActive(true);
        choicesPanel.SetActive(true);
        isShowingChoices = true;
    }

void SelectChoice(string targetLabel)
{
    choicesPanel.SetActive(false);
    isShowingChoices = false;

    if (JumpTo(targetLabel))
        ReadNextLine();
}

    System.Collections.IEnumerator ChangeBackgroundWithFade(Sprite newBackground)
    {
        isChangingBackground = true;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);

            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            // Плавно затемняем экран
            while (color.a < 1f)
            {
                color.a += Time.deltaTime / fadeDuration;
                color.a = Mathf.Clamp01(color.a);
                fadeImage.color = color;

                yield return null;
            }
        }

        // Меняем фон, пока экран чёрный
        backgroundImage.sprite = newBackground;

        if (fadeImage != null)
        {
            Color color = fadeImage.color;

            // Плавно убираем затемнение
            while (color.a > 0f)
            {
                color.a -= Time.deltaTime / fadeDuration;
                color.a = Mathf.Clamp01(color.a);
                fadeImage.color = color;

                yield return null;
            }

            color.a = 0f;
            fadeImage.color = color;
        }

        isChangingBackground = false;
    }

void HideUI()
{
    // Скрываем изображение диалогового окна
    Transform dialogueImage = transform.Find("DialogueImage");
    if (dialogueImage != null)
        dialogueImage.gameObject.SetActive(false);

    // Скрываем имя персонажа
    if (characterName != null)
        characterName.gameObject.SetActive(false);

    // Скрываем текст диалога
    if (dialogueText != null)
        dialogueText.gameObject.SetActive(false);

    // FadeImage не трогаем — затемнение останется
}
}
