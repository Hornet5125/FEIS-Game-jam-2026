using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PixelRenderOutput : MonoBehaviour
{
    [SerializeField] private RenderTexture pixelTexture;

    private Camera renderCamera;
    private Camera outputCamera;
    private GameObject outputObject;

    private void Awake()
    {
        renderCamera = GetComponent<Camera>();

        if (pixelTexture == null)
        {
            Debug.LogError("PixelRenderOutput: назначь RT_HorrorPixel!");
            enabled = false;
            return;
        }

        // Исходная камера рисует сцену в низком разрешении.
        renderCamera.targetTexture = pixelTexture;

        // Создаём отдельную камеру для вывода текстуры на экран.
        outputObject = new GameObject("PixelOutputCamera");
        outputCamera = outputObject.AddComponent<Camera>();

        outputCamera.clearFlags = CameraClearFlags.SolidColor;
        outputCamera.backgroundColor = Color.black;
        outputCamera.cullingMask = 0;
        outputCamera.orthographic = true;
        outputCamera.depth = renderCamera.depth + 1;

        outputObject.AddComponent<PixelOutputBlitter>()
            .Initialize(pixelTexture, outputCamera);

        outputCamera.allowHDR = false;
        outputCamera.allowMSAA = false;
    }

    private void OnDestroy()
    {
        if (renderCamera != null &&
            renderCamera.targetTexture == pixelTexture)
        {
            renderCamera.targetTexture = null;
        }

        if (outputObject != null)
            Destroy(outputObject);
    }
}

public class PixelOutputBlitter : MonoBehaviour
{
    private RenderTexture source;
    private Camera outputCamera;
    private Material material;

    public void Initialize(RenderTexture texture, Camera cam)
    {
        source = texture;
        outputCamera = cam;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material == null)
        {
            Shader shader = Shader.Find("Unlit/Texture");

            if (shader == null)
            {
                Graphics.Blit(source, dest);
                return;
            }

            material = new Material(shader);
        }

        Graphics.Blit(source, dest, material);
    }

    private void OnDestroy()
    {
        if (material != null)
            Destroy(material);
    }
}
