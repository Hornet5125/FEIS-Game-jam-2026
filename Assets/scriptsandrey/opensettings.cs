using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Dropdown qualityDropdown;

    void Start()
    {
        // Подписываемся на изменения
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume; // Меняем общую громкость
    }

    void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex); // Меняем качество
    }
}