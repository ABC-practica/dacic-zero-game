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
    [SerializeField] [Range(0.001f, 0.2f)] private float softness = 0.03f;
    [SerializeField] [Range(0f, 1f)] private float minLuminance = 0f;
    [SerializeField] [Range(1f, 10f)] private float contrast = 3f;
    [SerializeField] [Range(0f, 1f)] private float ditherAmount = 0.5f;

    [Header("Supersampling (render bigger, then downsample)")]
    [SerializeField] private bool useSupersampling = false;
    [SerializeField] [Range(1f, 4f)] private float supersampleFactor = 2f;

    [Header("Optional low-res downsample (chunkier look)")]
    [SerializeField] private bool useLowRes = false;
    [SerializeField] [Range(0.1f, 1f)] private float resolutionScale = 0.5f;

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
        ditherMaterial.SetFloat("_Softness", softness);
        ditherMaterial.SetFloat("_MinLum", minLuminance);
        ditherMaterial.SetFloat("_Contrast", contrast);
        ditherMaterial.SetFloat("_DitherAmount", ditherAmount);

        // Supersampling and low-res downsample are mutually exclusive —
        // supersampling makes dots finer, low-res makes them chunkier.
        if (useSupersampling)
        {
            int highW = Mathf.RoundToInt(src.width * supersampleFactor);
            int highH = Mathf.RoundToInt(src.height * supersampleFactor);

            RenderTexture high = RenderTexture.GetTemporary(highW, highH, 0, src.format);
            high.filterMode = FilterMode.Bilinear;

            // Upscale the source into the high-res buffer, dither at that resolution,
            // then let the final Blit to dst downsample it back to screen size.
            Graphics.Blit(src, high, ditherMaterial);
            Graphics.Blit(high, dst);

            RenderTexture.ReleaseTemporary(high);
        }
        else if (useLowRes)
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
