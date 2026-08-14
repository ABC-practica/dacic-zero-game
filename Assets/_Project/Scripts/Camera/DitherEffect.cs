using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class DitherEffect : MonoBehaviour
{
    [SerializeField] private Shader ditherShader;
    [SerializeField] private Texture2D noiseTex;
    [SerializeField] private Color darkColor = Color.black;
    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private float noiseScale = 4f;

    [Header("Optional low-res downsample")]
    [SerializeField] private bool useLowRes = false;
    [SerializeField][Range(0.1f, 1f)] private float resolutionScale = 0.5f;

    private Material ditherMaterial;

    void OnEnable()
    {
        if (ditherShader == null)
            ditherShader = Shader.Find("Hidden/ObraDither");

        if (ditherShader != null && ditherMaterial == null)
        {
            ditherMaterial = new Material(ditherShader);
            ditherMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void OnDisable()
    {
        if (ditherMaterial != null)
        {
            DestroyImmediate(ditherMaterial);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (ditherMaterial == null || noiseTex == null)
        {
            Graphics.Blit(src, dst);
            return;
        }

        ditherMaterial.SetTexture("_NoiseTex", noiseTex);
        ditherMaterial.SetColor("_DarkColor", darkColor);
        ditherMaterial.SetColor("_LightColor", lightColor);
        ditherMaterial.SetFloat("_NoiseScale", noiseScale);

        if (useLowRes)
        {
            int lowW = Mathf.Max(1, Mathf.RoundToInt(src.width * resolutionScale));
            int lowH = Mathf.Max(1, Mathf.RoundToInt(src.height * resolutionScale));

            RenderTexture low = RenderTexture.GetTemporary(lowW, lowH, 0, src.format);
            low.filterMode = FilterMode.Point;

            Graphics.Blit(src, low, ditherMaterial);
            Graphics.Blit(low, dst);

            RenderTexture.ReleaseTemporary(low);
        }
        else
        {
            Graphics.Blit(src, dst, ditherMaterial);
        }
    }
}