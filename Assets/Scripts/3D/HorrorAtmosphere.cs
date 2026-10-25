using UnityEngine;
using UnityEngine.Rendering;

public class HorrorAtmosphere : MonoBehaviour
{
    [Header("Fog")]
    [SerializeField] private Color fogColor =
        new Color(0.055f, 0.065f, 0.075f);

    [SerializeField] private float fogDensity = 0.04f;

    [Header("Ambient Lighting")]
    [SerializeField] private Color ambientColor =
        new Color(0.12f, 0.12f, 0.15f);

    private void Awake()
    {
        // Включаем атмосферный туман.
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;

        // Уменьшаем яркость окружающего освещения.
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
    }
}
