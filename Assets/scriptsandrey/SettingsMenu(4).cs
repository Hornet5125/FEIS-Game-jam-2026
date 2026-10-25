using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;

    private readonly List<Resolution> resolutions = new List<Resolution>();
    private Coroutine resolutionRoutine;

    private const string VolumeKey = "MasterVolume";
    private const string WidthKey = "ResolutionWidth";
    private const string HeightKey = "ResolutionHeight";

    private void Awake()
    {
        // Every launch starts at the monitor's native/current desktop resolution.
        if (!Application.isEditor)
        {
            Resolution monitor = Screen.currentResolution;
            Screen.SetResolution(monitor.width, monitor.height, FullScreenMode.FullScreenWindow);
        }
    }

    private void Start()
    {
        float volume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));
        AudioListener.volume = volume;

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.SetValueWithoutNotify(volume);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (resolutionDropdown != null)
        {
            PopulateResolutions();
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }
        else
        {
            Debug.LogWarning("SettingsMenu: Resolution Dropdown (TMP) is not assigned.", this);
        }
    }

    private void PopulateResolutions()
    {
        resolutions.Clear();
        var names = new List<string>();
        resolutionDropdown.ClearOptions();

        foreach (Resolution res in Screen.resolutions)
        {
            if (resolutions.Exists(r => r.width == res.width && r.height == res.height))
                continue;
            resolutions.Add(res);
            names.Add(res.width + " x " + res.height);
        }

        if (resolutions.Count == 0)
        {
            var fallback = Screen.currentResolution;
            resolutions.Add(fallback);
            names.Add(fallback.width + " x " + fallback.height);
        }

        resolutionDropdown.AddOptions(names);
        int selected = resolutions.FindIndex(r => r.width == Screen.width && r.height == Screen.height);
        if (selected < 0)
            selected = resolutions.FindIndex(r => r.width == Screen.currentResolution.width &&
                                                  r.height == Screen.currentResolution.height);
        resolutionDropdown.SetValueWithoutNotify(selected >= 0 ? selected : 0);
        resolutionDropdown.RefreshShownValue();
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    public void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= resolutions.Count) return;

        Resolution chosen = resolutions[index];
        PlayerPrefs.SetInt(WidthKey, chosen.width);
        PlayerPrefs.SetInt(HeightKey, chosen.height);
        PlayerPrefs.Save();

        // Let the dropdown clean up its temporary popup/blocker before resizing Windows.
        resolutionDropdown.Hide();
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (resolutionRoutine != null) StopCoroutine(resolutionRoutine);
        resolutionRoutine = StartCoroutine(ApplyResolution(chosen.width, chosen.height));
    }

    private IEnumerator ApplyResolution(int width, int height)
{
    yield return new WaitForSecondsRealtime(0.35f);

    if (!Application.isEditor &&
        (Screen.width != width ||
         Screen.height != height ||
         Screen.fullScreenMode != FullScreenMode.FullScreenWindow))
    {
        Screen.SetResolution(
            width,
            height,
            FullScreenMode.FullScreenWindow
        );
    }

    resolutionRoutine = null;
}

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
    }
}
