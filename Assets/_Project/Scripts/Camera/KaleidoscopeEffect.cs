using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class KaleidoscopeEffect : MonoBehaviour
{
    [SerializeField] private Shader kaleidoscopeShader;
    [SerializeField] [Range(2, 24)] private int segments = 6;
    [SerializeField] [Range(0f, 1f)] private float centerX = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float centerY = 0.5f;
    [SerializeField] private float rotation = 0f;
    [SerializeField] private bool autoRotate = false;
    [SerializeField] private float autoRotateSpeed = 0.3f;
    [SerializeField] [Range(0.1f, 5f)] private float zoom = 1f;
    [SerializeField] [Range(0f, 0.5f)] private float centerRadius = 0.1f;
    [SerializeField] [Range(0.001f, 0.3f)] private float featherWidth = 0.05f;

    private Material kaleidoscopeMaterial;
    private float currentRotation;

    void OnEnable()
    {
        if (kaleidoscopeShader == null)
            kaleidoscopeShader = Shader.Find("Hidden/Kaleidoscope");

        if (kaleidoscopeShader != null && kaleidoscopeMaterial == null)
        {
            kaleidoscopeMaterial = new Material(kaleidoscopeShader);
            kaleidoscopeMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void OnDisable()
    {
        if (kaleidoscopeMaterial != null)
        {
            DestroyImmediate(kaleidoscopeMaterial);
        }
    }

    void Update()
    {
        if (autoRotate)
        {
            currentRotation += autoRotateSpeed * Time.deltaTime;
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (kaleidoscopeMaterial == null)
        {
            Graphics.Blit(src, dst);
            return;
        }

        float finalRotation = rotation + (autoRotate ? currentRotation : 0f);

        kaleidoscopeMaterial.SetFloat("_Segments", segments);
        kaleidoscopeMaterial.SetFloat("_CenterX", centerX);
        kaleidoscopeMaterial.SetFloat("_CenterY", centerY);
        kaleidoscopeMaterial.SetFloat("_Rotation", finalRotation);
        kaleidoscopeMaterial.SetFloat("_Zoom", zoom);
        kaleidoscopeMaterial.SetFloat("_CenterRadius", centerRadius);
        kaleidoscopeMaterial.SetFloat("_FeatherWidth", featherWidth);

        Graphics.Blit(src, dst, kaleidoscopeMaterial);
    }
}
