using UnityEngine;
using UnityEditor;
using System.IO;

public class BayerTextureGenerator
{
    // Standard 8x8 Bayer matrix, values 0-63
    static readonly int[,] bayer8x8 = new int[8, 8]
    {
        { 0, 32, 8, 40, 2, 34, 10, 42 },
        { 48, 16, 56, 24, 50, 18, 58, 26 },
        { 12, 44, 4, 36, 14, 46, 6, 38 },
        { 60, 28, 52, 20, 62, 30, 54, 22 },
        { 3, 35, 11, 43, 1, 33, 9, 41 },
        { 51, 19, 59, 27, 49, 17, 57, 25 },
        { 15, 47, 7, 39, 13, 45, 5, 37 },
        { 63, 31, 55, 23, 61, 29, 53, 21 }
    };

    [MenuItem("Tools/Obra Dither/Generate Bayer 8x8 Texture")]
    public static void GenerateBayerTexture()
    {
        int size = 8;
        Texture2D tex = new Texture2D(size, size, TextureFormat.R8, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float value = bayer8x8[y, x] / 64f; // normalize to 0-1
                tex.SetPixel(x, y, new Color(value, value, value, 1));
            }
        }
        tex.Apply();

        byte[] pngData = tex.EncodeToPNG();
        string path = "Assets/BayerTexture8x8.png";
        File.WriteAllBytes(path, pngData);
        AssetDatabase.Refresh();

        // Set import settings: point filter, repeat wrap, no compression
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.filterMode = FilterMode.Point;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        Debug.Log("Bayer texture generated at " + path);
    }
}