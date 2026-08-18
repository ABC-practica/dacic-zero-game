using UnityEngine;
using UnityEditor;
using System.IO;

public class BlueNoiseGenerator
{
    private const int Size = 64;      // texture width/height
    private const int Radius = 3;     // gaussian kernel half-width
    private const float Sigma = 1.5f;

    [MenuItem("Tools/Obra Dither/Generate Blue Noise 64x64 Texture")]
    public static void GenerateBlueNoiseTexture()
    {
        int n = Size * Size;

        // Precompute toroidal Gaussian kernel weights
        int kernelDim = Radius * 2 + 1;
        float[,] kernel = new float[kernelDim, kernelDim];
        for (int dx = -Radius; dx <= Radius; dx++)
        {
            for (int dy = -Radius; dy <= Radius; dy++)
            {
                float d2 = dx * dx + dy * dy;
                kernel[dx + Radius, dy + Radius] = Mathf.Exp(-d2 / (2f * Sigma * Sigma));
            }
        }

        float[,] energy = new float[Size, Size];
        int[,] rank = new int[Size, Size];
        bool[,] on = new bool[Size, Size];

        // Greedy void-filling: repeatedly place the next point in the
        // largest remaining "void" (lowest energy = furthest from existing points).
        for (int r = 0; r < n; r++)
        {
            int bestX = 0, bestY = 0;
            float bestEnergy = float.MaxValue;

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (on[x, y]) continue;
                    if (energy[x, y] < bestEnergy)
                    {
                        bestEnergy = energy[x, y];
                        bestX = x;
                        bestY = y;
                    }
                }
            }

            on[bestX, bestY] = true;
            rank[bestX, bestY] = r;

            // Add this point's repulsion contribution to nearby cells (toroidal wrap)
            for (int dx = -Radius; dx <= Radius; dx++)
            {
                for (int dy = -Radius; dy <= Radius; dy++)
                {
                    int nx = ((bestX + dx) % Size + Size) % Size;
                    int ny = ((bestY + dy) % Size + Size) % Size;
                    energy[nx, ny] += kernel[dx + Radius, dy + Radius];
                }
            }
        }

        Texture2D tex = new Texture2D(Size, Size, TextureFormat.R8, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Repeat;

        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                float value = rank[x, y] / (float)(n - 1);
                tex.SetPixel(x, y, new Color(value, value, value, 1));
            }
        }
        tex.Apply();

        byte[] pngData = tex.EncodeToPNG();
        string path = "Assets/BlueNoiseTexture64x64.png";
        File.WriteAllBytes(path, pngData);
        AssetDatabase.Refresh();

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = false;
            importer.SaveAndReimport();
        }

        Debug.Log("Blue noise texture generated at " + path);
    }
}
